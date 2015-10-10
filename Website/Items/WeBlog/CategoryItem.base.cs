using CustomItemGenerator.Fields.SimpleTypes;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public partial class CategoryItem : CustomItem
    {

        public static readonly string TemplateId = "{61FF8D49-90D7-4E59-878D-DF6E03400D3B}";


        #region Boilerplate CustomItem Code

        public CategoryItem(Item innerItem) : base(innerItem)
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

        #endregion //Boilerplate CustomItem Code


        #region Field Instance Methods


        public CustomTextField Title
        {
            get
            {
                return new CustomTextField(InnerItem, InnerItem.Fields["Title"]);
            }
        }


        #endregion //Field Instance Methods
    }
}