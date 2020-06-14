using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class NewCategory : CreateItem
    {
        protected ICategoryManager CategoryManager { get; set; }

        public NewCategory(IBlogManager blogManager, BaseItemManager itemManager, ICategoryManager categoryManager)
           : base(blogManager, itemManager)
        {
            Assert.ArgumentNotNull(categoryManager, nameof(categoryManager));

            CategoryManager = categoryManager;
        }

        protected override ID GetTemplateId(BlogHomeItem blogItem)
        {
            return blogItem.BlogSettings.CategoryTemplateID;
        }

        protected override Item GetParentItem(BlogHomeItem blogItem)
        {
            return CategoryManager.GetCategoryRoot(blogItem);
        }
    }
}