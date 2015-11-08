using System;
using Sitecore.Buckets.Util;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Buckets
{
    public class SimpleCreationDateBucketFolderPath : IDynamicBucketFolderPath
    {
        [Obsolete]
        public string GetFolderPath(ID newItemId, ID parentItemId, DateTime creationDateOfNewItem)
        {
            return creationDateOfNewItem.Year + "/" + creationDateOfNewItem.Month;
        }

        public string GetFolderPath(Database database, string name, ID templateId, ID newItemId, ID parentItemId,
                                    DateTime creationDateOfNewItem)
        {
            return creationDateOfNewItem.Year + "/" + creationDateOfNewItem.Month;
        }
    }
}