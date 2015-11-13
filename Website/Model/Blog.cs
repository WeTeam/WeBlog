using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class Blog : BlogRenderingModelBase
    {
        public Item BlogItem { get; set; }
        public string Hyperlink { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            BlogItem = currentBlog;
            Hyperlink = currentBlog.SafeGet(x => x.Url);
        }
    }
}