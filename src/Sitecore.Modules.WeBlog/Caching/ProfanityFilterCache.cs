using Sitecore.Caching;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Caching
{
    public class ProfanityFilterCache : SimpleCache, IWeBlogCache
    {
        private Database _database = null;

        public ProfanityFilterCache(string name, long maxSize) : base(name, maxSize)
        {
            _database = ContentHelper.GetContentDatabase();
        }

        public ProfanityFilterCache(ICache innerCache, Database database) : base(innerCache)
        {
            _database = database ?? ContentHelper.GetContentDatabase();
        }

        protected string CacheName
        {
            get
            {
                var dbName = _database != null ? _database.Name : "nodb";
                return "wordlist_" + dbName;
            }
        }

        public IEnumerable<string> WordList
        {
            get
            {
                var cachedList = Get(CacheName);
                if (!string.IsNullOrEmpty(cachedList))
                {
                    return cachedList.Split('|').ToList();
                }
                return null;
            }
            set
            {
                Set(CacheName, string.Join("|", value));
            }
        }

        public void ClearCache()
        {
            InnerCache.Clear();
        }
    }
}