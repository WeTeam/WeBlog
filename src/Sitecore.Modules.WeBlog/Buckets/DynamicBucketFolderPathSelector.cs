using System;
using Sitecore.Buckets.Extensions;
using Sitecore.Buckets.Util;
using Sitecore.Data;
using Sitecore.StringExtensions;

namespace Sitecore.Modules.WeBlog.Buckets
{
    public class DynamicBucketFolderPathSelector : IDynamicBucketFolderPath
    {
        public string GetFolderPath(Database database, string name, ID templateId, ID newItemId, ID parentItemId,
                                    DateTime creationDateOfNewItem)
        {
            if (database == null)
                return string.Empty;

            var parentItem = database.GetItem(parentItemId);
            if (parentItem != null)
            {
                // Ensure we're at the root of the bucket
                parentItem = parentItem.GetParentBucketItemOrRootOrSelf();
                // The following is a Sitecore config path query, not a Sitecore content tree path
                var xpath = "/sitecore/WeBlog/DynamicBucketFolderPathSelector/*[includeTemplates/*[text() = '{0}'] or paths/*[text() = '{1}']]/handler"
                    .FormatWith(
                        parentItem.TemplateID, parentItem.Paths.FullPath.ToLower());
                var configNode = Sitecore.Configuration.Factory.GetConfigNode(xpath);

                if (configNode != null)
                {
                    var handler = Sitecore.Configuration.Factory.CreateObject<IDynamicBucketFolderPath>(configNode);
                    if (handler != null)
                    {
                        return (handler as IDynamicBucketFolderPath).GetFolderPath(database, name, templateId, newItemId,
                                                                                   parentItemId, creationDateOfNewItem);
                    }
                }
            }

            return new BucketFolderPathResolver().GetFolderPath(database, name, templateId, newItemId, parentItemId, creationDateOfNewItem);
        }
    }
}