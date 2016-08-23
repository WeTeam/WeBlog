using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class FileItem : CustomItem
    {
        public FileItem(Item innerItem) : base(innerItem) { }

        public static implicit operator FileItem(Item innerItem)
        {
            return innerItem != null ? new FileItem(innerItem) : null;
        }

        public static implicit operator Item(FileItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public string Url
        {
            get
            {
                return InnerItem[Fields.Url];
            }
        }

        public static class Fields
        {
            public const string Url = "Url";
        }
    }
}