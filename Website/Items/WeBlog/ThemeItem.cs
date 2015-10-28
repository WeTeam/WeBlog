using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Items.Custom;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public class ThemeItem : CustomItem
    {
        public ThemeItem(Item innerItem) : base(innerItem) { }

        public static implicit operator ThemeItem(Item innerItem)
        {
            return innerItem != null ? new ThemeItem(innerItem) : null;
        }

        public static implicit operator Item(ThemeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public CustomTextField FileLocation
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["File Location"]); }
        }
    }
}