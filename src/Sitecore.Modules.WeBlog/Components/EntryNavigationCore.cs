using Sitecore.Data;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Components
{
    public class EntryNavigationCore : IEntryNavigationCore
    {
        protected IEntryManager EntryManager { get; }

        protected IBlogManager BlogManager { get; }

        public EntryNavigationCore(IBlogManager blogManager, IEntryManager entryManager)
        {
            BlogManager = blogManager;
            EntryManager = entryManager;
        }

        public EntryItem GetPreviousEntry(EntryItem entry)
        {
            var criteria = new EntryCriteria
            {
                MaximumDate = entry.EntryDate.DateTime,
                PageNumber = 1,
                PageSize = 2
            };

            var blogHomeItem = BlogManager.GetCurrentBlog(entry);
            var entries = EntryManager.GetBlogEntries(blogHomeItem, criteria, ListOrder.Descending);

            if(entries.Results.Count > 1)
                return Database.GetItem(entries.Results[0].Uri);

            return null;
        }

        public EntryItem GetNextEntry(EntryItem entry)
        {
            var criteria = new EntryCriteria
            {
                MinimumDate = entry.EntryDate.DateTime,
                PageNumber = 1,
                PageSize = 2
            };

            var blogHomeItem = BlogManager.GetCurrentBlog(entry);
            var entries = EntryManager.GetBlogEntries(blogHomeItem, criteria, ListOrder.Ascending);

            if (entries.Results.Count > 1)
                return Database.GetItem(entries.Results[1].Uri);

            return null;
        }
    }
}