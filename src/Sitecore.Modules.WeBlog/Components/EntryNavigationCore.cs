using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;
using System;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Components
{
    public class EntryNavigationCore : IEntryNavigationCore
    {
        protected IEntryManager EntryManager { get; }

        protected IBlogManager BlogManager { get; }

        public EntryNavigationCore(IBlogManager blogManager, IEntryManager entryManager)
        {
            Assert.ArgumentNotNull(blogManager, nameof(blogManager));
            Assert.ArgumentNotNull(entryManager, nameof(entryManager));

            BlogManager = blogManager;
            EntryManager = entryManager;
        }

        public EntryItem GetPreviousEntry(EntryItem entry)
        {
            if (entry == null)
                return null;

            // Push the date to the next day to ensure we've covered all entries for that day.
            var entries = GetEntriesCloseToEntry(entry, ListOrder.Descending, x => x.MaximumDate = entry.EntryDate.DateTime.AddDays(1));

            if (entries != null && entries.Results.Any())
            {
                var previous = entries.Results.SkipWhile(x => x.Uri.ItemID != entry.ID).Skip(1).FirstOrDefault();
                if (previous == null)
                    return null;

                return GetItem(previous.Uri);
            }

            return null;
        }

        public EntryItem GetNextEntry(EntryItem entry)
        {
            if (entry == null)
                return null;

            // Push the date to the previous day to ensure we've covered all entries for that day.
            var entries = GetEntriesCloseToEntry(entry, ListOrder.Ascending, x => x.MinimumDate = entry.EntryDate.DateTime.AddDays(-1));

            if (entries != null && entries.Results.Any())
            {
                var next = entries.Results.SkipWhile(x => x.Uri.ItemID != entry.ID).Skip(1).FirstOrDefault();
                if (next == null)
                    return null;

                return GetItem(next.Uri);
            }
            
            return null;
        }

        protected SearchResults<Model.Entry> GetEntriesCloseToEntry(EntryItem entry, ListOrder resultOrder, Action<EntryCriteria> mutator)
        {
            var blogHomeItem = BlogManager.GetCurrentBlog(entry);
            if (blogHomeItem == null)
                return null;

            var pageSizes = new[] { 5, 50, 100, int.MaxValue };

            foreach (var pageSize in pageSizes)
            {
                var criteria = new EntryCriteria
                {
                    PageNumber = 1,
                    PageSize = pageSize
                };

                mutator.Invoke(criteria);

                var entries = EntryManager.GetBlogEntries(blogHomeItem, criteria, resultOrder);

                for(var i = 0; i < entries.Results.Count; i++)
                {
                    if(entries.Results[i].Uri.ItemID == entry.ID && i < entries.Results.Count - 1)
                        return entries;
                }
            }

            return null;
        }

        protected virtual Item GetItem(ItemUri uri)
        {
            return Database.GetItem(uri);
        }
    }
}