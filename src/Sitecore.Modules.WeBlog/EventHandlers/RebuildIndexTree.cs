using System;
using System.Linq;
using Sitecore.Buckets.Managers;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Jobs.AsyncUI;

namespace Sitecore.Modules.WeBlog.EventHandlers
{
    public class RebuildIndexTree
    {
        public void OnItemAdded(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            Item item = Event.ExtractParameter(args, 0) as Item;
            if (item == null || !BucketManager.IsBucketable(item) || IsPublish())
            {
                return;
            }

            Item bucketRoot = GetBucketRoot(item);
            if (bucketRoot != null && BucketManager.IsBucket(bucketRoot))
            {
                // do not remove ToArray() call
                IndexCustodian.RefreshTree((SitecoreIndexableItem)item).ToArray();
            }
        }

        private bool IsPublish()
        {
            return JobContext.IsJob && JobContext.Job.Category == "publish";
        }

        protected virtual Item GetBucketRoot(Item item)
        {
            var current = item;
            do
            {
                current = current.Parent;
            }
            while (current?.Parent != null && !BucketManager.IsBucket(current));

            return current;
        }
    }
}