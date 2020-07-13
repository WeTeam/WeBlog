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
    public class NewEntry : CreateItem
    {
        protected IBlogSettingsResolver BlogSettingsResolver { get; set; }

        public NewEntry()
            : this(
                  ServiceLocator.ServiceProvider.GetRequiredService<IBlogManager>(),
                  ServiceLocator.ServiceProvider.GetRequiredService<BaseItemManager>(),
                  ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>()
                  )
        {
        }

        public NewEntry(IBlogManager blogManager, BaseItemManager itemManager, IBlogSettingsResolver blogSettingsResolver)
            : base(blogManager, itemManager)
        {
            Assert.ArgumentNotNull(blogSettingsResolver, nameof(blogSettingsResolver));

            BlogSettingsResolver = blogSettingsResolver;
        }

        protected override ID GetTemplateId(BlogHomeItem blogItem)
        {
            var blogSettings = BlogSettingsResolver.Resolve(blogItem);
            return blogSettings.EntryTemplateID;
        }

        protected override Item GetParentItem(BlogHomeItem blogItem)
        {
            return blogItem;
        }
    }
}