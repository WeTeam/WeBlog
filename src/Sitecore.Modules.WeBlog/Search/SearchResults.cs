using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Search
{
    /// <summary>
    /// A set of search results.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class SearchResults<TResult>
    {
        /// <summary>
        /// Creates an empty search result.
        /// </summary>
        public static SearchResults<TResult> Empty => new SearchResults<TResult>(new List<TResult>(), false);

        /// <summary>
        /// Gets the items for the search result.
        /// </summary>
        public IList<TResult> Results { get; }

        /// <summary>
        /// Gets a value indicating whether there are more search results.
        /// </summary>
        public bool HasMoreResults { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="results">The items for the search result.</param>
        /// <param name="hasMoreResults">A value indicating whether there are more search results.</param>
        public SearchResults(IList<TResult> results, bool hasMoreResults)
        {
            Results = results;
            HasMoreResults = hasMoreResults;
        }
    }
}