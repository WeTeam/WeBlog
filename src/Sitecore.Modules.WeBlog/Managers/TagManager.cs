using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Comparers;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Managers
{
    public class TagManager : ITagManager
    {
		/// <summary>
		/// Gets or sets the <see cref="IEntryManager"/> instance used to access the blog structure.
        /// </summary>
        protected IEntryManager EntryManager { get; set; }
		
		public TagManager()
            : this(null)
        {
        }

        public TagManager(IEntryManager entryManager = null)
        {
            EntryManager = entryManager ?? ManagerFactory.EntryManagerInstance;
        }
		
		/// <summary>
        /// Gets the tags for the blog
        /// </summary>
        /// <param name="blogItem">The blog to get the tags for</param>
        /// <returns>An array of unique tags</returns>
        public Tag[] GetTagsForBlog(BlogHomeItem blog)
        {
            if (blog != null)
            {
                var entries = EntryManager.GetBlogEntries(blog.InnerItem);
                return ExtractAndSortTags(entries);
            }

            return new Tag[0];
        }
		
        /// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="entry">The entry to get the tags for</param>
        /// <returns>The sorted tags for the entry</returns>
        public Tag[] GetTagsForEntry(EntryItem entry)
        {
            return ExtractAndSortTags(new[] { entry });
        }

		/// <summary>
        /// Extracts the tags from a collection of <see cre="EntryItem"/>
        /// </summary>
        /// <param name="entries">The entries to get the tags from</param>
        /// <returns>The sorted tags</returns>
        protected virtual Tag[] ExtractAndSortTags(IEnumerable<EntryItem> entries)
        {
            var tags = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries)
            {
                if(entry == null)
                    continue;

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

        #region Deprecated
        /// <summary>
        /// Gets the tags for the blog by the given blog ID
        /// </summary>
        /// <param name="blogId">The ID of the blog to get the tags for</param>
        /// <returns>An array of unique tags</returns>
        [Obsolete("Use GetTagsByBlog(Item) instead")] // deprecated 3.0
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
        /// Gets the tags for the blog by the given blog item
        /// </summary>
        /// <param name="blogItem">The blog to get the tags for</param>
        /// <returns>An array of unique tags</returns>
		[Obsolete("Use GetTagsForBlog(BlogHomeItem) instead")] // deprecated 3.0
		public string[] GetTagsByBlog(Item blogItem)
		{
			return GetTagsForBlog(blogItem).Select(x => x.Name).ToArray();
		}
		
		/// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="Entry">The entry to get the tags for</param>
        /// <returns></returns>
		[Obsolete("Use GetTagsForEntry(EntryItem) instead")] // deprecated 3.0
        public Dictionary<string, int> GetTagsByEntry(EntryItem entry)
        {
            var tagList = new List<string>();

            if(entry != null)
                tagList.AddRange(entry.TagsSplit);

            return SortByWeight(tagList);
        }

        /// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <returns>A sorted array of tags with counts</returns>
        [Obsolete("Use GetTagsByBlog(Item) instead")] // deprecated 3.0
        public Dictionary<string, int> GetAllTags()
        {
            return GetAllTags(ManagerFactory.BlogManagerInstance.GetCurrentBlog());
        }
		
		/// <summary>
        /// Gets the tags and coutns for the blog by the given blog ID
        /// </summary>
        /// <param name="blog">The blog to get the tags for</param>
        /// <returns>A sorted array of tags with counts</returns>
		[Obsolete("Use GetTagsForBlog(BlogHomeItem) instead")] // deprecated 3.0
        public Dictionary<string, int> GetAllTags(BlogHomeItem blog)
		{
			return GetTagsForBlog(blog).ToDictionary(x => x.Name, x => x.Count);
		}
		
		/// <summary>
        /// Sort tags by the number of occurances
        /// </summary>
        /// <param name="tags">The tags to sort</param>
        /// <returns>A dictionary of tags with counts sorted by count</returns>
		[Obsolete("No longer used. Tags are returned sorted.")] // deprecated 3.0
        public Dictionary<string, int> SortByWeight(IEnumerable<string> tags)
        {
            var sort = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		    if (tags == null)
		        return sort;

            foreach (var tag in tags)
            {
                if(tag == null)
                    continue;

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
        #endregion
    }
}