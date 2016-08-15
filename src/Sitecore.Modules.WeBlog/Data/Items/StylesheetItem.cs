using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class StylesheetItem : FileItem
    {
        public static readonly ID TemplateId = new ID("{E12AA8E5-D615-4014-916C-7E482CC4E83E}");

        public StylesheetItem(Item innerItem) : base(innerItem) { }

        public static implicit operator StylesheetItem(Item innerItem)
        {
            return innerItem != null ? new StylesheetItem(innerItem) : null;
        }

        public static implicit operator Item(StylesheetItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }
    }
}