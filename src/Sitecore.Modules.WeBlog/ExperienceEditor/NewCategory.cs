using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class NewCategory : CreateItem
    {
        protected ICategoryManager CategoryManager { get; set; }

        protected IBlogSettingsResolver BlogSettingsResolver { get; set; }

        public NewCategory()
            : this(
                  ServiceLocator.ServiceProvider.GetRequiredService<IBlogManager>(),
                  ServiceLocator.ServiceProvider.GetRequiredService<BaseItemManager>(),
                  ServiceLocator.ServiceProvider.GetRequiredService<ICategoryManager>(),
                  ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>()
                  )
        {
        }

        public NewCategory(IBlogManager blogManager, BaseItemManager itemManager, ICategoryManager categoryManager, IBlogSettingsResolver blogSettingsResolver)
           : base(blogManager, itemManager)
        {
            Assert.ArgumentNotNull(categoryManager, nameof(categoryManager));
            Assert.ArgumentNotNull(blogSettingsResolver, nameof(blogSettingsResolver));

            CategoryManager = categoryManager;
            BlogSettingsResolver = blogSettingsResolver;
        }

        protected override ID GetTemplateId(BlogHomeItem blogItem)
        {
            var blogSettings = BlogSettingsResolver.Resolve(blogItem);
            return blogSettings.CategoryTemplateID;
        }

        protected override Item GetParentItem(BlogHomeItem blogItem)
        {
            return CategoryManager.GetCategoryRoot(blogItem);
        }
    }
}