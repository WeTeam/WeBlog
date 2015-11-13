using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
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
                    return (InterestingEntriesAlgorithm)Enum.Parse(typeof(InterestingEntriesAlgorithm), mode);
                }
                catch (ArgumentException ex)
                {
                    Log.Warn("Failed to parse Mode as InterestingEntriesAlgorithm: " + mode, ex);
                }
            }
            return defaultAlgorithm;
        }

        public virtual EntryItem[] GetEntries(Item blogItem, int maxCount)
        {
            switch (Algorithm)
            {
                case InterestingEntriesAlgorithm.Custom:
                case InterestingEntriesAlgorithm.Comments:
                    return EntryManagerInstance.GetPopularEntriesByComment(blogItem, maxCount);

                case InterestingEntriesAlgorithm.PageViews:
                    return EntryManagerInstance.GetPopularEntriesByView(blogItem, maxCount);
            }
            return new EntryItem[0];
        }
    }
}