using System;
using System.ComponentModel;
using Sitecore.Links;
using Sitecore.Sharedsource.Web.UI.Sublayouts;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar
{
    public partial class BlogFacebookLike : SublayoutBase
    {
        public string UrlToLike { get; set; }
        public string LayoutStyle { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool SendButton { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowFaces { get; set; }

        public int Width { get; set; }
        public string ColorScheme { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(string.IsNullOrEmpty(UrlToLike))
                UrlToLike = LinkManager.GetItemUrl(DataSourceItem, new UrlOptions { AlwaysIncludeServerUrl = true });
        }
    }
}