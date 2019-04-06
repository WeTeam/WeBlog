using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

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
                // TODO: Index tags as separate terms and use faceting rather than iterating every entry.
                var entries = EntryManager.GetBlogEntries(blog.InnerItem, EntryCriteria.AllEntries);
                return ExtractAndSortTags(entries);
            }

            return new Tag[0];
        }

        /// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="entry">The entry to get the tags for</param>
        /// <returns>The sorted tags for the entry</returns>
        public Tag[] GetTagsForEntry(EntryItem entryItem)
        {
            if(entryItem == null)
                return new Tag[0];

            var entry = new Entry
            {
                Tags = entryItem.TagsSplit,
                EntryDate = entryItem.EntryDate.DateTime
            };

            return ExtractAndSortTags(new[] { entry });
        }

        /// <summary>
        /// Gets the tags for a blog entry and sorts by weight
        /// </summary>
        /// <param name="entry">The entry to get the tags for</param>
        /// <returns>The sorted tags for the entry</returns>
        public Tag[] GetTagsForEntry(Entry entry)
        {
            return ExtractAndSortTags(new[] { entry });
        }

		/// <summary>
        /// Extracts the tags from a collection of <see cre="EntryItem"/>
        /// </summary>
        /// <param name="entries">The entries to get the tags from</param>
        /// <returns>The sorted tags</returns>
        protected virtual Tag[] ExtractAndSortTags(IEnumerable<Entry> entries)
        {
            var tags = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries)
            {
                if(entry?.Tags == null)
                    continue;

                var stringTags = entry.Tags.Distinct();
                foreach (var stringTag in stringTags)
                {
                    if (tags.ContainsKey(stringTag))
                    {
                        var tag = tags[stringTag];
                        tag.Count++;
                        if (entry.EntryDate > tag.LastUsed)
                        {
                            tag.LastUsed = entry.EntryDate;
                        }
                    }
                    else
                    {
                        tags.Add(stringTag, new Tag
                        {
                            Name = stringTag,
                            Count = 1,
                            LastUsed = entry.EntryDate
                        });
                    }
                }
            }
            var tagArray = tags.Values.OrderByDescending(x => x.Count).ToArray();
            return tagArray;
        }
    }
}