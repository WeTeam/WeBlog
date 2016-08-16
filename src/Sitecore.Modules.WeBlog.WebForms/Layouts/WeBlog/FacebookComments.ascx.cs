using System;
using Sitecore.Links;
using Sitecore.Sharedsource.Web.UI.Sublayouts;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    public partial class BlogFacebookComments : SublayoutBase
    {
        public string UrlToCommentOn { get; set; }
        public int NumberOfPosts { get; set; }
        public int Width { get; set; }
        public string ColorScheme { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (string.IsNullOrEmpty(UrlToCommentOn))
                UrlToCommentOn = LinkManager.GetItemUrl(DataSourceItem, new UrlOptions { AlwaysIncludeServerUrl = true });
        }
    }
}