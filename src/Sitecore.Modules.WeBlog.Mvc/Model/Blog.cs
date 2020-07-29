using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class Blog : BlogRenderingModelBase
    {
        protected IBlogManager BlogManager { get; }

        protected BaseLinkManager LinkManager { get; }

        public Item BlogItem { get; set; }
        
        public string Hyperlink { get; set; }

        public ThemeItem Theme { get; set; }

        public Blog(IBlogManager blogManager, BaseLinkManager linkManager)
        {
            Assert.ArgumentNotNull(blogManager, nameof(blogManager));
            Assert.ArgumentNotNull(linkManager, nameof(linkManager));

            BlogManager = blogManager;
            LinkManager = linkManager;
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            var currentBlog = BlogManager.GetCurrentBlog();
            BlogItem = currentBlog;

            Hyperlink = LinkManager.GetItemUrl(currentBlog);
            Theme = currentBlog.Theme.Item;
        }
    }
}