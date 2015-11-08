using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Managers
{
    public interface ITagManager
    {
        /// <summary>
        /// Gets the tags for the blog by the given blog ID
        /// </summary>
        /// <param name="blogId">The ID of the blog to get the tags for</param>
        /// <returns>An array of unique tags</returns>
        string[] GetTagsByBlog(ID blogId);

        /// <summary>
        /// Gets the tags for the blog by the given blog ID
        /// </summary>
        /// <param name="blogItem">The blog to get the tags for</param>
        /// <returns>An array of unique tags</returns>
        string[] GetTagsByBlog(Item blogItem);

        /// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="Entry">The entry to get the tags for</param>
        /// <returns></returns>
        Dictionary<string, int> GetTagsByEntry(EntryItem entry);

        /// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <returns>A sorted array of tags with counts</returns>
        Dictionary<string, int> GetAllTags();

        /// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <param name="blog">The blog to get the tags for</param>
        /// <returns>A sorted array of tags with counts</returns>
        Dictionary<string, int> GetAllTags(BlogHomeItem blog);

        /// <summary>
        /// Sort tags by the number of occurances
        /// </summary>
        /// <param name="tags">The tags to sort</param>
        /// <returns>A dictionary of tags with counts sorted by count</returns>
        Dictionary<string, int> SortByWeight(IEnumerable<string> tags);
    }
}
