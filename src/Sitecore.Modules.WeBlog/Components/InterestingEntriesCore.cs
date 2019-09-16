using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Components
{
    public class InterestingEntriesCore : IInterstingEntriesAlgorithm
    {
        protected IEntryManager EntryManagerInstance { get; set; }

        /// <summary>
        /// Gets or sets the algorithm to use for selecting items to display
        /// </summary>
        public InterestingEntriesAlgorithm Algorithm { get; set; }

        public InterestingEntriesCore(IEntryManager entryManagerInstance, InterestingEntriesAlgorithm algororithm)
        {
            EntryManagerInstance = entryManagerInstance;
            Algorithm = algororithm;
        }

        public static InterestingEntriesAlgorithm GetAlgororithmFromString(string mode, InterestingEntriesAlgorithm defaultAlgorithm = InterestingEntriesAlgorithm.Comments)
        {
            if (!string.IsNullOrEmpty(mode))
            {
                try
                {
                    return (InterestingEntriesAlgorithm)Enum.Parse(typeof(InterestingEntriesAlgorithm), mode, true);
                }
                catch (ArgumentException ex)
                {
                    Logger.Warn("Failed to parse Mode as InterestingEntriesAlgorithm: " + mode, ex);
                }
            }
            return defaultAlgorithm;
        }

        public virtual EntryItem[] GetEntries(Item blogItem, int maxCount)
        {
            IList<ItemUri> uris = new List<ItemUri>();

            switch (Algorithm)
            {
                case InterestingEntriesAlgorithm.Custom:
                case InterestingEntriesAlgorithm.Comments:
                    uris = EntryManagerInstance.GetPopularEntriesByComment(blogItem, maxCount);
                    break;

                case InterestingEntriesAlgorithm.PageViews:
                    uris = EntryManagerInstance.GetPopularEntriesByView(blogItem, maxCount);
                    break;
            }

            var entryItems = new List<EntryItem>();

            foreach (var uri in uris)
            {
                var item = Database.GetItem(uri);
                if(item != null)
                    entryItems.Add(new EntryItem(item));
            }

            return entryItems.ToArray();
        }
    }
}