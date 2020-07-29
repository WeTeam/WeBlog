using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Configuration
{
    public class BlogSettingsResolver : IBlogSettingsResolver
    {
        protected BaseTemplateManager TemplateManager { get; }

        protected IWeBlogSettings WeBlogSettings { get; }

        public BlogSettingsResolver(BaseTemplateManager templateManager, IWeBlogSettings weBlogSettings)
        {
            Assert.ArgumentNotNull(templateManager, nameof(templateManager));
            Assert.ArgumentNotNull(weBlogSettings, nameof(weBlogSettings));

            TemplateManager = templateManager;
            WeBlogSettings = weBlogSettings;
        }

        public BlogSettings Resolve(BlogHomeItem blogItem)
        {
            var blogSettings = new BlogSettings(WeBlogSettings);

            if (blogItem != null && TemplateManager.TemplateIsOrBasedOn(blogItem.InnerItem, WeBlogSettings.BlogTemplateIds))
            {
                blogSettings.CategoryTemplateID = blogItem.DefinedCategoryTemplate.Field.TargetID;
                blogSettings.EntryTemplateID = blogItem.DefinedEntryTemplate.Field.TargetID;
                blogSettings.CommentTemplateID = blogItem.DefinedCommentTemplate.Field.TargetID;
            }

            return blogSettings;
        }
    }
}
