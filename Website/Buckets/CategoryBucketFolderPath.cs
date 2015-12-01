using System;
using System.Linq;
using Sitecore.Buckets.Util;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Buckets
{
    public class CategoryBucketFolderPath : IDynamicBucketFolderPath
    {
        public string FieldName { set; get; }
        public string Default { set; get; }

        [Obsolete]
        public string GetFolderPath(ID newItemId, ID parentItemId, DateTime creationDateOfNewItem)
        {
            return GetFolderPath(Context.ContentDatabase, null, null, newItemId, parentItemId, creationDateOfNewItem);
        }

        public string GetFolderPath(Database database, string name, ID templateId, ID newItemId, ID parentItemId, DateTime creationDateOfNewItem)
        {
            var innerItem = database.GetItem(newItemId);
            if (innerItem != null)
            {
                var field = (MultilistField)innerItem.Fields[FieldName];
                if (field != null)
                {
                    var firstCategory = GetFirstCategoryItem(field);
                    if (firstCategory != null)
                    {
                        return firstCategory.Name;
                    }
                }
            }
            return Default ?? "Uncategorized";
        }

        private static Item GetFirstCategoryItem(MultilistField field)
        {
            return field.GetItems().FirstOrDefault();
        }
    }
}