using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Sharedsource.Web.UI.Sublayouts;
using System;
using System.ComponentModel;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar
{
    [AllowDependencyInjection]
    public partial class BlogFacebookLike : SublayoutBase
    {
        protected BaseLinkManager LinkManager { get; }

        public string UrlToLike { get; set; }
        public string LayoutStyle { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool SendButton { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowFaces { get; set; }

        public int Width { get; set; }
        public string ColorScheme { get; set; }

        public BlogFacebookLike(BaseLinkManager linkManager)
        {
            LinkManager = linkManager ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseLinkManager>();
        }

        public BlogFacebookLike()
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(string.IsNullOrEmpty(UrlToLike))
                UrlToLike = LinkManager.GetAbsoluteItemUrl(DataSourceItem);
        }
    }
}