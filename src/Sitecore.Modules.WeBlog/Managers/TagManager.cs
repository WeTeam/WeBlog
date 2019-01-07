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
    }
}