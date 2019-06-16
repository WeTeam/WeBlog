using System.Collections.Generic;
using Sitecore.Abstractions;
using Sitecore.Caching.Generics;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Caching
{
    /// <summary>
    /// A cache for entries.
    /// </summary>
    public class EntrySearchCache : IEntrySearchCache, IWeBlogCache
    {
        /// <summary>
        /// The name of the cache used for caching entries.
        /// </summary>
        public const string CacheName = "WeBlog.Entries";

        /// <summary>
        /// The cache to store the entries in.
        /// </summary>
        private readonly ICache<EntrySearchCacheKey> _cache = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="cacheManager">The <see cref="BaseCacheManager"/> used to retieve the internal cache.</param>
        /// <param name="settings">The settings to operate with.</param>
        public EntrySearchCache(BaseCacheManager cacheManager = null, IWeBlogSettings settings = null)
        {
            _cache = GetEntriesCache(cacheManager, settings);
        }

        /// <summary>
        /// Gets the list of entries for the specified <see cref="EntryCriteria"/>.
        /// </summary>
        /// <param name="criteria">The criteria used to search for the entries.</param>
        /// <param name="resultOrder">The ordering of the results.</param>
        /// <returns>The list of entries for the criteria, or null if the criteria has not been cached.</returns>
        public List<Entry> Get(EntryCriteria criteria, ListOrder resultOrder)
        {
            Assert.ArgumentNotNull(criteria, nameof(criteria));

            var key = new EntrySearchCacheKey(criteria, resultOrder);
            return (SearchResults<Entry>)_cache.GetValue(key);
        }

        /// <summary>
        /// Sets the list of entries for the specified <see cref="EntryCriteria"/>.
        /// </summary>
        /// <param name="criteria">The criteria used to search for the entries.</param>
        /// <param name="resultOrder">The ordering of the results.</param>
        /// <param name="entries">The entries for the search criteria.</param>
        public void Set(EntryCriteria criteria, ListOrder resultOrder, SearchResults<Entry> entries)
        {
            Assert.ArgumentNotNull(criteria, nameof(criteria));
            Assert.ArgumentNotNull(entries, nameof(entries));

            var key = new EntrySearchCacheKey(criteria, resultOrder);
            _cache.Add(criteria, key);
        }

        /// <summary>
        /// Clear all entries from the cache.
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Gets the entries cache.
        /// </summary>
        /// <param name="cacheManager">The <see cref="BaseCacheManager"/> used to retieve the internal cache.</param>
        /// <param name="settings">The settings to operate with.</param>
        /// <returns></returns>
        private ICache<EntryCriteria> GetEntriesCache(BaseCacheManager cacheManager, IWeBlogSettings settings)
        {
            var cacheSize = (settings ?? WeBlogSettings.Instance).EntriesCacheSize;

            if (cacheManager != null)
                return cacheManager.GetNamedInstance<EntrySearchCacheKey>(CacheName, cacheSize, true);
            
            return Sitecore.Caching.CacheManager.GetNamedInstance<EntrySearchCacheKey>(CacheName, cacheSize, true);
        }
    }
}