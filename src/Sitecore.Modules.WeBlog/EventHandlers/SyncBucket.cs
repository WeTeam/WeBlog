using System;
using Sitecore.Buckets.Managers;
using Sitecore.Buckets.Util;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Exceptions;
using Sitecore.Jobs.AsyncUI;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Reflection;

namespace Sitecore.Modules.WeBlog.EventHandlers
{
    public class SyncBucket
    {
        public void OnItemSaved(object sender, EventArgs args)
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
                if (!IsAlreadyOnItsPlace(item, bucketRoot))
                {
                    BucketManager.MoveItemIntoBucket(item, bucketRoot);
                }
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
            while (current != null && current.Parent != null && !BucketManager.IsBucket(current));

            return current;
        }

        protected virtual bool IsAlreadyOnItsPlace(Item itemToCheck, Item bucketRoot)
        {
            return itemToCheck.Parent.Paths.Path.Equals(GetDestinationFolderPath(bucketRoot, itemToCheck.Statistics.Created, itemToCheck), StringComparison.OrdinalIgnoreCase);
        }

        protected virtual string GetDestinationFolderPath(Item topParent, DateTime childItemCreationDateTime, Item itemToMove)
        {
            Assert.ArgumentNotNull(topParent, "topParent");
            Assert.ArgumentNotNull(childItemCreationDateTime, "childItemCreationDateTime");
            Assert.ArgumentNotNull(itemToMove, "itemToMove");
            Type type = Type.GetType(BucketConfigurationSettings.DynamicBucketFolderPath);
            IDynamicBucketFolderPath bucketFolderPath = ReflectionUtil.CreateObject(type) as IDynamicBucketFolderPath;
            if (bucketFolderPath == null)
            {
                Logger.Error("Could not instantiate DynamicBucketFolderPath of type " + type, this);
                throw new ConfigurationException("Could not instantiate DynamicBucketFolderPath of type " + type);
            }

            Database database = topParent.Database;
            string str = bucketFolderPath.GetFolderPath(database, itemToMove.Name, itemToMove.TemplateID, itemToMove.ID, topParent.ID, childItemCreationDateTime);

            if (BucketConfigurationSettings.BucketFolderPath == string.Empty && bucketFolderPath is DateBasedFolderPath)
                str = "Repository";
            return String.IsNullOrEmpty(str)
                ? topParent.Paths.FullPath
                : topParent.Paths.FullPath + Sitecore.Buckets.Util.Constants.ContentPathSeperator + str;
        }
    }
}