using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Fields;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public partial class CategoryItem : CustomItem
    {
        public CategoryItem(Item innerItem)
            : base(innerItem)
        {
        }

        public static implicit operator CategoryItem(Item innerItem)
        {
            return innerItem != null ? new CategoryItem(innerItem) : null;
        }

        public static implicit operator Item(CategoryItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public CustomTextField Title
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Title"]); }
        }

        public string DisplayTitle
        {
            get { return !string.IsNullOrEmpty(Title.Text) ? Title.Text : Name; }
        }
    }
}