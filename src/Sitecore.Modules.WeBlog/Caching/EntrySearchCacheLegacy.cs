using System.Collections.Generic;
using Sitecore.Caching.Generics;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog;
using Sitecore.Modules.WeBlog.Caching;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

public class EntrySearchCache : IEntrySearchCache, IWeBlogCache
{
    private Cache<EntrySearchCacheKey, EntryList> _cache = null;

    /// <summary>
    /// The name of the cache used for caching entries.
    /// </summary>
    public const string CacheName = "WeBlog.Entries";

    /// <summary>
    /// Create a new instance.
    /// </summary>
    /// <param name="settings">The settings to operate with.</param>
    public EntrySearchCache(IWeBlogSettings settings = null)
    {
        var cacheSize = (settings ?? WeBlogSettings.Instance).EntriesCacheSize;
        _cache = new Cache<EntrySearchCacheKey, EntryList>(CacheName, cacheSize)
        {
            Enabled = true
        };
    }

    /// <summary>
    /// Gets the list of entries for the specified <see cref="EntryCriteria"/>.
    /// </summary>
    /// <param name="criteria">The criteria used to search for the entries.</param>
    /// <param name="resultOrder">The ordering of the results.</param>
    /// <returns>The list of entries for the criteria, or null if the criteria has not been cached.</returns>
    public SearchResults<Entry> Get(EntryCriteria criteria, ListOrder resultOrder)
    {
        Assert.ArgumentNotNull(criteria, nameof(criteria));

        var key = new EntrySearchCacheKey(criteria, resultOrder);
        var cacheEntry = _cache.GetEntry(key, true);
        return cacheEntry?.Data.Entries;
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
        _cache.Add(key, new EntryList() { Entries = entries });
    }

    /// <summary>
    /// Clear all entries from the cache.
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }
}