using System.ComponentModel;
using System.Web.Mvc;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogFacebookLikeController : BlogBaseController
    {
        public string UrlToLike { get; set; }
        public string LayoutStyle { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool SendButton { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowFaces { get; set; }

        public int Width { get; set; }
        public string ColorScheme { get; set; }

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(UrlToLike))
            {
                UrlToLike = LinkManager.GetItemUrl(DataSourceItem, new UrlOptions { AlwaysIncludeServerUrl = true });
            }

            var model = new FacebookLikeRenderingModel
            {
                UrlToLike = UrlToLike,
                LayoutStyle = LayoutStyle,
                SendButton = SendButton,
                ShowFaces = ShowFaces,
                Width = Width,
                ColorScheme = ColorScheme
            };

            return View("~/Views/WeBlog/FacebookLike.cshtml", model);
        }
    }
}