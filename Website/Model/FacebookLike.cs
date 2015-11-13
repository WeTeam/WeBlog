using System.ComponentModel;
using Sitecore.Links;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class FacebookLike : BlogRenderingModelBase
    {
        public string UrlToLike { get; set; }
        public string LayoutStyle { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool SendButton { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowFaces { get; set; }

        public int Width { get; set; }
        public string ColorScheme { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            if (string.IsNullOrEmpty(UrlToLike))
            {
                UrlToLike = LinkManager.GetItemUrl(DataSourceItem, new UrlOptions { AlwaysIncludeServerUrl = true });
            }
        }
    }
}