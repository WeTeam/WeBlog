using System;
using Sitecore.Buckets.Util;
using Sitecore.Data;
using Sitecore.Data.Fields;

namespace Sitecore.Modules.WeBlog.Buckets
{
    public class SimpleCreationDateBucketFolderPath : IDynamicBucketFolderPath
    {
        public string Format { set; get; }
        public string FieldName { set; get; }

        [Obsolete]
        public string GetFolderPath(ID newItemId, ID parentItemId, DateTime creationDateOfNewItem)
        {
            return creationDateOfNewItem.Year + "/" + creationDateOfNewItem.Month;
        }

        public string GetFolderPath(Database database, string name, ID templateId, ID newItemId, ID parentItemId,
                                    DateTime creationDateOfNewItem)
        {
            var innerItem = database.GetItem(newItemId);
            if (innerItem != null)
            {
                var field = (DateField) innerItem.Fields[FieldName];
                if (field!=null)
                {
                    creationDateOfNewItem = field.DateTime;
                }
            }
            return creationDateOfNewItem.ToString(Format ?? BucketConfigurationSettings.BucketFolderPath, Context.Culture);
        }
    }
}