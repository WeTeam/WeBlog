using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Caching
{
    /// <summary>
    /// Defines a cache for entries.
    /// </summary>
    public interface IEntrySearchCache
    {
        /// <summary>
        /// Gets the list of entries for the specified <see cref="EntryCriteria"/>.
        /// </summary>
        /// <param name="criteria">The criteria used to search for the entries.</param>
        /// <returns>The list of entries for the criteria, or null if the criteria has not been cached.</returns>
        List<Entry> Get(EntryCriteria criteria);

        /// <summary>
        /// Sets the list of entries for the specified <see cref="EntryCriteria"/>.
        /// </summary>
        /// <param name="criteria">The criteria used to search for the entries.</param>
        /// <param name="entries">The entries for the search criteria.</param>
        void Set(EntryCriteria criteria, List<Entry> entries);
    }
}
