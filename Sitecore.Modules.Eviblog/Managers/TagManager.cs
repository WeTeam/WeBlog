using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Modules.Eviblog.Comparers;
using Sitecore.Modules.Eviblog.Items;

namespace Sitecore.Modules.Eviblog.Managers
{
    public class TagManager
    {
        /// <summary>
        /// Gets the tags by blog ID.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <returns></returns>
        public static List<string> GetTagsByBlogID(ID BlogID)
        {
            var TagList = new List<string>();

            return TagList;
        }

        /// <summary>
        /// Gets the tags by entry.
        /// </summary>
        /// <param name="Entry">The entry.</param>
        /// <returns></returns>
        public static Dictionary<string, int> GetTagsByEntry(Entry Entry)
        {
            var TagList = new List<string>();

            TagList.AddRange(Entry.Tags);

            return SortByWeight(TagList);
        }

        /// <summary>
        /// Gets all the tags.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetAllTags()
        {
            var TagList = new List<string>();

            Blog currentBlog = BlogManager.GetCurrentBlog();

            foreach (Entry entry in EntryManager.GetBlogEntries(currentBlog.ID))
            {
                TagList.AddRange(entry.Tags);
            }

            return SortByWeight(TagList);
        }

        /// <summary>
        /// Sorts the by weight.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        private static Dictionary<string, int> SortByWeight(IEnumerable<string> tags)
        {
            var sort = new Dictionary<string, int>();

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