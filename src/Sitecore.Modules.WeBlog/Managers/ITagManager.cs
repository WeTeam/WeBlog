using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;
using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Managers
{
    public interface ITagManager
    {
        /// <summary>
        /// Gets the tags for the blog
        /// </summary>
        /// <param name="blogItem">The blog to get the tags for</param>
        /// <returns>An array of unique tags</returns>
        Tag[] GetTagsForBlog(BlogHomeItem blogItem);

        /// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="entry">The entry to get the tags for</param>
        /// <returns></returns>
        Tag[] GetTagsForEntry(EntryItem entry);
    }
}
