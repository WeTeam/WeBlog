using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Sharedsource.Web.UI.Sublayouts;
using System;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    [AllowDependencyInjection]
    public partial class BlogFacebookComments : SublayoutBase
    {
        protected BaseLinkManager _linkManager = null;

        public string UrlToCommentOn { get; set; }

        public int NumberOfPosts { get; set; }

        public int Width { get; set; }

        public string ColorScheme { get; set; }

        public BlogFacebookComments(BaseLinkManager linkManager)
        {
            _linkManager = linkManager;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            if (string.IsNullOrEmpty(UrlToCommentOn))
                UrlToCommentOn = _linkManager.GetAbsoluteItemUrl(DataSourceItem);
        }
    }
}