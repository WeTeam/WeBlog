using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Comparers;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Managers
{
    public class TagManager : ITagManager
    {
        /// <summary>
        /// Gets the tags for the blog by the given blog ID
        /// </summary>
        /// <param name="blogId">The ID of the blog to get the tags for</param>
        /// <returns>An array of unique tags</returns>
        public string[] GetTagsByBlog(ID blogId)
        {
            if (blogId != (ID)null)
            {
                var blogItem = Sitecore.Context.Database.GetItem(blogId);
                if (blogItem != null)
                    return GetTagsByBlog(blogItem);
            }
            
            return new string[0];
        }

        /// <summary>
        /// Gets the tags for the blog by the given blog ID
        /// </summary>
        /// <param name="blogItem">The blog to get the tags for</param>
        /// <returns>An array of unique tags</returns>
        public string[] GetTagsByBlog(Item blogItem)
        {
            var tagList = new List<string>();
            var entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(blogItem);

            foreach (var entry in entries)
            {
                foreach (var tag in entry.TagsSplit)
                {
                    if (!tagList.Contains(tag))
                        tagList.Add(tag);
                }
            }

            return tagList.ToArray();
        }

        /// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="Entry">The entry to get the tags for</param>
        /// <returns></returns>
        public Dictionary<string, int> GetTagsByEntry(EntryItem entry)
        {
            var tagList = new List<string>();

            tagList.AddRange(entry.TagsSplit);

            return SortByWeight(tagList);
        }

        /// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <returns>A sorted array of tags with counts</returns>
        public Dictionary<string, int> GetAllTags()
        {
            return GetAllTags(ManagerFactory.BlogManagerInstance.GetCurrentBlog());
        }

        /// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <param name="blog">The blog to get the tags for</param>
        /// <returns>A sorted array of tags with counts</returns>
        public Dictionary<string, int> GetAllTags(BlogHomeItem blog)
        {
            var tagList = new List<string>();

            if (blog != null)
            {
                var entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(blog.InnerItem);

                foreach (var entry in entries)
                {
                    tagList.AddRange(entry.TagsSplit.Distinct());
                }
            }

            return SortByWeight(tagList);
        }

        /// <summary>
        /// Sort tags by the number of occurances
        /// </summary>
        /// <param name="tags">The tags to sort</param>
        /// <returns>A dictionary of tags with counts sorted by count</returns>
        public Dictionary<string, int> SortByWeight(IEnumerable<string> tags)
        {
            var sort = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var tag in tags)
            {
                if (sort.ContainsKey(tag))
                {
                    ++sort[tag];
                }
                else
                {
                    sort.Add(tag, 1);
                }
            }

            var array = sort.ToArray();
            Array.Sort(array, new TagWeightComparer());

            return sort;
        }
    }
}