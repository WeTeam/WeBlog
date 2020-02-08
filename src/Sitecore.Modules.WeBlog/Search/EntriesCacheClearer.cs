using System;
using Sitecore.Abstractions;
using Sitecore.ContentSearch;
using Sitecore.Events;
using Sitecore.Modules.WeBlog.Caching;
using Sitecore.Modules.WeBlog.Configuration;

namespace Sitecore.Modules.WeBlog.Search
{
    /// <summary>
    /// An event handler to clear the WeBlog entries cache.
    /// </summary>
    public class EntriesCacheClearer
    {
        protected IWeBlogSettings Settings = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public EntriesCacheClearer()
            : this(null)
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="settings">The settings to use.</param>
        public EntriesCacheClearer(IWeBlogSettings settings)
        {
            Settings = settings ?? WeBlogSettings.Instance;
        }

        /// <summary>
        /// Event handler for the indexing:end event.
        /// </summary>
        /// <param name="sender">The object which triggered the event.</param>
        /// <param name="args">The arguments for the event.</param>
        public void OnIndexingEnd(object sender, EventArgs args)
        {
#if SC93
            var indexName = Event.ExtractParameter<string>(args, 0);
#else
            var indexName = ContentSearchManager.Locator.GetInstance<IEvent>().ExtractParameter<string>(args, 0);
#endif

            if (indexName.StartsWith(Settings.SearchIndexName))
            {
                var cache = CacheManager.GetCache<EntrySearchCache>(EntrySearchCache.CacheName);
                cache?.ClearCache();
            }
        }
    }
}