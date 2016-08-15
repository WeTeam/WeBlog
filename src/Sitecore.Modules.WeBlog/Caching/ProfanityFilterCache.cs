using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Caching
{
    public class ProfanityFilterCache : SimpleCache, IWeBlogCache
    {
        public ProfanityFilterCache(string name, long maxSize) : base(name, maxSize) { }

        protected string CacheName
        {
            get { return "wordlist_" + Context.Database.Name; }
        }

        public IEnumerable<string> WorList
        {
            get
            {
                var cachedList = Get(CacheName);
                if (!String.IsNullOrEmpty(cachedList))
                {
                    return cachedList.Split('|').ToList();
                }
                return null;
            }
            set
            {
                Set(CacheName, String.Join("|", value));
            }
        }

        public void ClearCache()
        {
            InnerCache.Clear();
        }
    }
}