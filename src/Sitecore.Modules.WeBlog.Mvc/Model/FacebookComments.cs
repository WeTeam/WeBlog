using Sitecore.Abstractions;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class FacebookComments : BlogRenderingModelBase
    {
        protected BaseLinkManager LinkManager { get; }

        public string UrlToCommentOn { get; set; }
        public int NumberOfPosts { get; set; }
        public int Width { get; set; }
        public string ColorScheme { get; set; }

        public FacebookComments(BaseLinkManager linkManager)
        {
            Assert.ArgumentNotNull(linkManager, nameof(linkManager));

            LinkManager = linkManager;
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            if (string.IsNullOrEmpty(UrlToCommentOn))
            {
                UrlToCommentOn = LinkManager.GetAbsoluteItemUrl(DataSourceItem);
            }
        }
    }
}