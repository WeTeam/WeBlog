using System;
using Sitecore.Search;

namespace Sitecore.Modules.WeBlog.Utilities
{
    public static class Search
    {
        public static Index GetSearchIndex()
        {
            return SearchManager.GetIndex(Constants.Index.Name);
        }

        /// <summary>
        /// Transforms an input to remove the whitespace and allow tokenising on other characters
        /// </summary>
        /// <param name="value">The string to transform</param>
        /// <returns>The transformed string</returns>
        public static string TransformCSV(string value)
        {
            var collapsed = value.Replace(" ", string.Empty);
            return collapsed.Replace(',', ' ');
        }
    }
}