using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Fields;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public partial class CategoryItem : CustomItem
    {

        public static readonly string TemplateId = "{61FF8D49-90D7-4E59-878D-DF6E03400D3B}";

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

        /// <summary>
        /// Gets the URL for a category
        /// </summary>
        /// <returns>The URL of the category</returns>
        public string GetUrl()
        {
            return InnerItem.GetUrl();
        }
    }
}