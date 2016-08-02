using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Managers
{
    public class TagManager : ITagManager
    {
        /// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="entry">The entry to get the tags for</param>
        /// <returns></returns>
        public Tag[] GetTagsByEntry(EntryItem entry)
        {
            return ExtractAndSortTags(new[] { entry });
        }

        /// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <returns>A sorted array of tags with counts</returns>
        public Tag[] GetAllTags()
        {
            return GetAllTags(ManagerFactory.BlogManagerInstance.GetCurrentBlog());
        }

        /// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <param name="blog">The blog to get the tags for</param>
        /// <returns>A sorted array of tags with counts</returns>
        public Tag[] GetAllTags(BlogHomeItem blog)
        {
            if (blog != null)
            {
                var entries = ManagerFactory.EntryManagerInstance.GetBlogEntries(blog.InnerItem);
                return ExtractAndSortTags(entries);
            }

            return new Tag[0];
        }

        protected virtual Tag[] ExtractAndSortTags(IEnumerable<EntryItem> entries)
        {
            var tags = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries)
            {
                var stringTags = entry.TagsSplit.Distinct();
                foreach (var stringTag in stringTags)
                {
                    if (tags.ContainsKey(stringTag))
                    {
                        var tag = tags[stringTag];
                        tag.Count++;
                        if (entry.EntryDate.DateTime > tag.LastUsed)
                        {
                            tag.LastUsed = entry.EntryDate.DateTime;
                        }
                    }
                    else
                    {
                        tags.Add(stringTag, new Tag
                        {
                            Name = stringTag,
                            Count = 1,
                            LastUsed = entry.EntryDate.DateTime
                        });
                    }
                }
            }
            var tagArray = tags.Values.OrderByDescending(x => x.Count).ToArray();
            return tagArray;
        }

    }
}