using System;
using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Diagnostics;

namespace Sitecore.Modules.WeBlog.Caching
{
    public class CacheManager
    {
        protected static Dictionary<string, IWeBlogCache> Caches = new Dictionary<string, IWeBlogCache>();

        public static ProfanityFilterCache ProfanityFilterCache
        {
            get
            {
                string cacheName = "WeBlog [profanity list]";
                var profanityList = GetCache<ProfanityFilterCache>(cacheName);
                if (profanityList == null)
                {
                    profanityList = new ProfanityFilterCache(cacheName,
                        StringUtil.ParseSizeString(Settings.ProfanityFilterCacheSize));
                    SetCache(cacheName, profanityList);
                }
                return profanityList;
            }
        }

        public static TranslatorCache TranslatorCache
        {
            get
            {
                string cacheName = "WeBlog [translator]";
                var translatorCache = GetCache<TranslatorCache>(cacheName);
                if (translatorCache == null)
                {
                    translatorCache = new TranslatorCache(cacheName);
                    SetCache(cacheName, translatorCache);
                }
                return translatorCache;
            }
        }

        public static T GetCache<T>(string key)
        {
            lock (Caches)
            {
                if (Caches.ContainsKey(key))
                {
                    return (T)Caches[key];
                }
            }
            return default(T);
        }

        public static void SetCache(string key, IWeBlogCache cache)
        {
            lock (Caches)
            {
                if (Caches.ContainsKey(key))
                {
                    Caches[key] = cache;
                }
                else
                {
                    Caches.Add(key, cache);
                }
            }
        }

        public static void ClearAllCaches()
        {
            lock (Caches)
            {
                foreach (KeyValuePair<string, IWeBlogCache> cache in Caches)
                {
                    Logger.Info(String.Format("Clearing {0} cache", cache.Key));
                    cache.Value.ClearCache();
                }
            }
        }
    }
}