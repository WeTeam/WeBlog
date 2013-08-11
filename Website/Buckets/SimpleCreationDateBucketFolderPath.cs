// This file is conditionally included in the project for Sitecore versions supporting item buckets (currently Sitecore 7.0)

using System;
using Sitecore.Buckets.Util;

namespace Sitecore.Modules.WeBlog.Buckets
{
    public class SimpleCreationDateBucketFolderPath : IDynamicBucketFolderPath
    {
        public string GetFolderPath(Data.ID newItemId, Data.ID parentItemId, DateTime creationDateOfNewItem)
        {
            return creationDateOfNewItem.Year + "/" + creationDateOfNewItem.Month;
        }
    }
}