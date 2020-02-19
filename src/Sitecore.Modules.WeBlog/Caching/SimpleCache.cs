﻿using Sitecore.Caching;

namespace Sitecore.Modules.WeBlog.Caching
{
    public class SimpleCache : CustomCache
    {
        public SimpleCache(string name, long maxSize) : base(name, maxSize) { }

#if FEATURE_ABSTRACTIONS
        public SimpleCache(ICache innerCache) : base(innerCache) { }
#endif

        public string Get(string cacheKey)
        {
            return GetString(cacheKey);
        }
        public void Set(string cacheKey, string value)
        {
            SetString(cacheKey, value);
        }
    }
}