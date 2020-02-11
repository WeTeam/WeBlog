using System.Collections.Generic;
using System.Linq;
using Sitecore.Caching;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Caching
{
    public class TranslatorCache : IWeBlogCache
    {
        #region Properties
        protected const string DefaultCacheSize = "500KB";
        protected string CacheNamePrefix = "Translator";
        private const string Key = "Key";
        private static ID _cacheRootId;
        private IWeBlogSettings _settings;

        protected Dictionary<string, Cache> Caches = new Dictionary<string, Cache>();

        public TranslatorCache() { }

        public TranslatorCache(string cacheName, IWeBlogSettings settings = null)
        {
            CacheNamePrefix = cacheName;
            _settings = settings ?? WeBlogSettings.Instance;
        }

        protected string CacheName
        {
            get { return CacheNamePrefix + "_" + _cacheRootId + "_" + Context.Database.Name; }
        }
        #endregion

        /// <summary>
        /// Clears all dictionary Caches.
        /// </summary>
        public void ClearCache()
        {
            //a bit heavy handed for now, can get more granular if performance need is there
            lock (Caches)
            {
                foreach (Cache cache in Caches.Values)
                {
                    cache.Clear();
                }
            }
        }

        /// <summary>
        /// Finds the dictionary entry.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Item FindEntry(string key)
        {
            Assert.ArgumentNotNull(key, "key");
            var dictionary = FindCache();
            if (dictionary != null && dictionary.ContainsKey(key))
            {
                ID entryId = dictionary[key] as ID;
                return Context.Database.GetItem(entryId);
            }
            return null;
        }

        /// <summary>
        /// Finds the dictionary cache.
        /// </summary>
        /// <returns></returns>
        protected Cache FindCache()
        {
            var dictionaryItem = ManagerFactory.BlogManagerInstance.GetDictionaryItem();
            if (dictionaryItem != null)
            {
                _cacheRootId = dictionaryItem.ID;
                Cache siteDictionary;
                lock (Caches)
                {
                    if (Caches.ContainsKey(CacheName))
                    {
                        siteDictionary = Caches[CacheName];
                    }
                    else
                    {
                        long cacheSize = StringUtil.ParseSizeString(_settings.GlobalizationCacheSize);
                        siteDictionary = new Cache(CacheName, cacheSize);
                        Caches[CacheName] = siteDictionary;
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

            return null;
        }

        /// <summary>
        /// Populates the cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        protected void PopulateCache(Cache cache)
        {
            Item dictionaryItem = ManagerFactory.BlogManagerInstance.GetDictionaryItem();
            _cacheRootId = dictionaryItem.ID;
            IEnumerable<Item> entries = dictionaryItem.Axes.GetDescendants();
            entries = entries.Where(entry => entry.TemplateID == _settings.DictionaryEntryTemplateId);
            foreach (Item entry in entries)
            {
                string key = entry[Key].Trim();
                if (!cache.ContainsKey(key))
                {
                    cache.Add(key, entry.ID);
                }
            }
        }
    }
}