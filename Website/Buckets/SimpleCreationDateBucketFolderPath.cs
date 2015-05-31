// This file is conditionally included in the project for Sitecore versions supporting item buckets
#if FEATURE_ITEM_BUCKETS
using System;
using Sitecore.Buckets.Util;

namespace Sitecore.Modules.WeBlog.Buckets
{
    public class SimpleCreationDateBucketFolderPath : IDynamicBucketFolderPath
    {
        [Obsolete]
        public string GetFolderPath(Data.ID newItemId, Data.ID parentItemId, DateTime creationDateOfNewItem)
        {
            return creationDateOfNewItem.Year + "/" + creationDateOfNewItem.Month;
        }

        public string GetFolderPath(Data.Database database, string name, Data.ID templateId, Data.ID newItemId, Data.ID parentItemId,
                                    DateTime creationDateOfNewItem)
        {
            return creationDateOfNewItem.Year + "/" + creationDateOfNewItem.Month;
        }
    }
}
#endif