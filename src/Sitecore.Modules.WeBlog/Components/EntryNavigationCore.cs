using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Components
{
    public class EntryNavigationCore : IEntryNavigationCore
    {
        protected IPostListCore PostListCore { get; set; }

        protected List<EntryItem> EntryItems { get; set; }

        public EntryNavigationCore(IPostListCore postListCore)
        {
            PostListCore = postListCore;
            EntryItems = PostListCore.Entries.Reverse().ToList();
        }

        public EntryItem GetPreviousEntry(EntryItem entry)
        {
            var currentEntry = EntryItems.FirstOrDefault(item => item.ID.Equals(entry.ID));
            var currentEntryIndex = EntryItems.IndexOf(currentEntry);
            if (currentEntryIndex > 0)
            {
                return EntryItems[currentEntryIndex - 1];
            }
            return null;
        }

        public EntryItem GetNextEntry(EntryItem entry)
        {
            var currentEntry = EntryItems.FirstOrDefault(item => item.ID.Equals(entry.ID));
            var currentEntryIndex = EntryItems.IndexOf(currentEntry);
            if (currentEntryIndex < EntryItems.Count - 1)
            {
                return EntryItems[currentEntryIndex + 1];
            }
            return null;
        }
    }
}