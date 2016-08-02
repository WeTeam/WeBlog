using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Managers
{
    public interface ITagManager
    {
        /// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="entry">The entry to get the tags for</param>
        /// <returns></returns>
        Tag[] GetTagsByEntry(EntryItem entry);

        /// <summary>
        /// Gets the tags and coutns for the current blog
        /// </summary>
        /// <returns>An array of tags sorted by count</returns>
        Tag[] GetAllTags();

        /// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <param name="blog">The blog to get the tags for</param>
        /// <returns>An array of tags sorted by count</returns>
        Tag[] GetAllTags(BlogHomeItem blog);
    }
}
