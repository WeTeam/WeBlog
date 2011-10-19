using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Caching;
using Sitecore.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Data.Managers;

namespace Sitecore.Modules.WeBlog.Globalization
{
    public class Translator
    {
        #region Properties
        protected const string DEFAULT_CACHE_SIZE = "500KB";
        protected const string CACHE_NAME_PREFIX = "Translator";
        private const string KEY = "Key";
        private const string PHRASE = "Phrase";
        private static ID _cacheRootID;

        protected static Dictionary<string, Cache> caches = new Dictionary<string, Cache>();

        protected static string CacheName
        {
            get
            {
                return CACHE_NAME_PREFIX + "_" + _cacheRootID + "_" + Sitecore.Context.Database.Name;
            }
        }
        #endregion

        /// <summary>
        /// Renders the specified key as text
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Text(string key)
        {
            var entry = FindEntry(key);
            if (entry == null)
            {
                return "#" + key + "#";
            }
            return entry[PHRASE];
        }

        /// <summary>
        /// Renders the specified key with a fieldrenderer.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Render(string key)
        {
            var entry = FindEntry(key);
            if (entry == null)
            {
                return "#" + key + "#";
            }
            return FieldRenderer.Render(entry, PHRASE);
        }

        public static string Render(string key, bool disableWebEditing)
        {
            var entry = FindEntry(key);
            if (entry == null)
            {
                return "#" + key + "#";
            }

            if (disableWebEditing)
                return FieldRenderer.Render(entry, PHRASE, "disable-web-editing=true");
            else
                return FieldRenderer.Render(entry, PHRASE);
        }

        /// <summary>
        /// Renders the specified key as a formatted string
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="arg0">The arg0.</param>
        /// <returns></returns>
        public static string Format(string key, object arg0)
        {
            string text = Text(key);
            return string.Format(text, arg0);
        }

        /// <summary>
        /// Renders the specified key as a formatted string
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string Format(string key, params object[] args)
        {
            string text = Text(key);
            return string.Format(text, args);
        }

        /// <summary>
        /// Clears all dictionary caches.
        /// </summary>
        public static void ClearCaches()
        {
            //a bit heavy handed for now, can get more granular if performance need is there
            foreach (Cache cache in caches.Values)
            {
                cache.Clear();
            }
        }

        /// <summary>
        /// Finds the dictionary entry.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected static Item FindEntry(string key)
        {
            Assert.ArgumentNotNull(key, "key");
            var dictionary = FindCache();
            if (dictionary.ContainsKey(key))
            {
                ID entryID = dictionary[key] as ID;
                return Sitecore.Context.Database.GetItem(entryID);
            }
            return null;
        }

        /// <summary>
        /// Finds the dictionary cache.
        /// </summary>
        /// <returns></returns>
        protected static Cache FindCache()
        {
            var dictionaryItem = BlogManager.GetDictionaryItem();
            if (dictionaryItem != null)
            {
                _cacheRootID = dictionaryItem.ID;
                Cache siteDictionary = null;
                lock (caches)
                {
                    if (caches.ContainsKey(CacheName))
                    {
                        siteDictionary = caches[CacheName];
                    }
                    else
                    {
                        string cacheSizeStr = Sitecore.Configuration.Settings.GetSetting(Settings.GlobalizationCacheSize, DEFAULT_CACHE_SIZE);
                        long cacheSize = Sitecore.StringUtil.ParseSizeString(cacheSizeStr);
                        siteDictionary = new Cache(CacheName, cacheSize);
                        caches[CacheName] = siteDictionary;
                    }
                }
                lock (siteDictionary)
                {
                    //do an initial load if the cache is empty
                    if (siteDictionary.Count == 0)
                    {
                        PopulateCache(siteDictionary);
                    }
                }
                return siteDictionary;
            }

            return new Cache(0);
        }

        /// <summary>
        /// Populates the cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        protected static void PopulateCache(Cache cache)
        {
            Item dictionaryItem = BlogManager.GetDictionaryItem();
            _cacheRootID = dictionaryItem.ID;
            if (dictionaryItem == null)
            {
                Log.Error("No dictionary configured for blog " + BlogManager.GetCurrentBlog().Name, typeof(Translator));
                return;
            }

            IEnumerable<Item> entries = dictionaryItem.Axes.GetDescendants();
            entries = entries.Where(entry => entry.TemplateID == Settings.DictionaryEntryTemplateId);
            foreach (Item entry in entries)
            {
                string key = entry[KEY].Trim();
                if (!cache.ContainsKey(key))
                {
                    cache.Add(key, entry.ID);
                }
            }
        }
    }
}