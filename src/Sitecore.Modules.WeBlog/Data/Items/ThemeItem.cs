using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class ThemeItem : CustomItem
    {
        public ThemeItem(Item innerItem)
            : base(innerItem)
        {
        }

        public string Credits
        {
            get { return InnerItem["Credit Markup"]; }
        }

        public static implicit operator ThemeItem(Item innerItem)
        {
            return innerItem != null ? new ThemeItem(innerItem) : null;
        }

        public static implicit operator Item(ThemeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }
    }
}