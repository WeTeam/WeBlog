using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Search.ComputedFields
{
    /// <summary>
    /// A computed index field which computes the closet entry item to the item being indexed.
    /// </summary>
    public class ClosestEntry : AbstractComputedIndexField
    {
        protected IEntryManager EntryManager = null;

        public ClosestEntry()
            : this(null)
        {
        }

        public ClosestEntry(IEntryManager entryManager)
        {
            EntryManager = entryManager ?? ManagerFactory.EntryManagerInstance;
        }

        public override object ComputeFieldValue(IIndexable indexable)
        {
            var indexableItem = indexable as SitecoreIndexableItem;
            if (indexableItem == null)
                return null;

            var entryItem = EntryManager.GetBlogEntryItemByCommentUri(indexableItem.Item.Uri);
            return entryItem?.InnerItem.Uri;
        }
    }
}