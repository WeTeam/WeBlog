<<<<<<< HEAD
﻿using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Managers;

=======
﻿using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

>>>>>>> Refactor EE requests to use common base class.
namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class NewCategory : CreateItem
    {
<<<<<<< HEAD
        protected IBlogSettingsResolver BlogSettingsResolver { get; }

        public NewCategory(IBlogSettingsResolver blogSettingsResolver)
        {
            Assert.ArgumentNotNull(blogSettingsResolver, nameof(blogSettingsResolver));

            BlogSettingsResolver = blogSettingsResolver;
        }

        public NewCategory()
            : this(ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>())
        {
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            var itemTitle = RequestContext.Argument;
            if (ItemUtil.IsItemNameValid(itemTitle))
            {
                var currentItem = RequestContext.Item;
                var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(currentItem);
                if (currentBlog != null)
                {
                    var settings = BlogSettingsResolver.Resolve(currentBlog);
                    var template = new TemplateID(settings.CategoryTemplateID);
                    var categories = ManagerFactory.CategoryManagerInstance.GetCategoryRoot(currentItem);
                    var newItem = ItemManager.AddFromTemplate(itemTitle, template, categories);
=======
        protected ICategoryManager CategoryManager { get; set; }

        public NewCategory(IBlogManager blogManager, BaseItemManager itemManager, ICategoryManager categoryManager)
           : base(blogManager, itemManager)
        {
            Assert.ArgumentNotNull(categoryManager, nameof(categoryManager));

            CategoryManager = categoryManager;
        }
>>>>>>> Refactor EE requests to use common base class.

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