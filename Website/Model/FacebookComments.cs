using Sitecore.Links;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class FacebookComments : BlogRenderingModelBase
    {
        public string UrlToCommentOn { get; set; }
        public int NumberOfPosts { get; set; }
        public int Width { get; set; }
        public string ColorScheme { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            if (string.IsNullOrEmpty(UrlToCommentOn))
            {
                UrlToCommentOn = LinkManager.GetItemUrl(DataSourceItem, new UrlOptions { AlwaysIncludeServerUrl = true });
            }
        }
    }
}