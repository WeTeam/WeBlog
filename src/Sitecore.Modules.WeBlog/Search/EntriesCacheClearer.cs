using System;
using Sitecore.Abstractions;
using Sitecore.ContentSearch;
using Sitecore.Modules.WeBlog.Caching;
using Sitecore.Modules.WeBlog.Configuration;
using CacheManager = Sitecore.Caching.CacheManager;

namespace Sitecore.Modules.WeBlog.Search
{
    /// <summary>
    /// An event handler to claer the WeBlog entries cache.
    /// </summary>
    public class EntriesCacheClearer
    {
        protected IWeBlogSettings Settings = null;

        public EntriesCacheClearer()
            : this(null)
        {
        }

        public EntriesCacheClearer(IWeBlogSettings settings)
        {
            Settings = settings ?? WeBlogSettings.Instance;
        }

        public void OnIndexingEnd(object sender, EventArgs args)
        {
            var indexName = ContentSearchManager.Locator.GetInstance<IEvent>().ExtractParameter<string>(args, 0);

            if (indexName.StartsWith(Settings.SearchIndexName))
            {
                var cache = CacheManager.GetNamedInstance<EntryCriteria>(EntrySearchCache.CacheName, 0, false);
                if(cache != null)
                {
                    cache.Clear();
                }
            }
        }
    }
}