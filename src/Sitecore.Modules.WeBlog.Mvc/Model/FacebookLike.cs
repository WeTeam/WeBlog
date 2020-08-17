using Sitecore.Abstractions;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Mvc.Presentation;
using System.ComponentModel;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class FacebookLike : BlogRenderingModelBase
    {
        protected BaseLinkManager _linkManager = null;

        public string UrlToLike { get; set; }
        public string LayoutStyle { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool SendButton { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowFaces { get; set; }

        public int Width { get; set; }
        public string ColorScheme { get; set; }

        public FacebookLike(BaseLinkManager linkManager)
        {
            _linkManager = linkManager;
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);

            if (string.IsNullOrEmpty(UrlToLike))
                UrlToLike = _linkManager.GetAbsoluteItemUrl(DataSourceItem);
        }
    }
}