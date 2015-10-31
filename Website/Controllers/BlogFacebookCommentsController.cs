using System.Web.Mvc;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogFacebookCommentsController : BlogBaseController
    {
        public string UrlToCommentOn { get; set; }
        public int NumberOfPosts { get; set; }
        public int Width { get; set; }
        public string ColorScheme { get; set; }

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(UrlToCommentOn))
            {
                UrlToCommentOn = LinkManager.GetItemUrl(DataSourceItem, new UrlOptions { AlwaysIncludeServerUrl = true });
            }

            var model = new FacebookCommentsRenderingModel
            {
                UrlToCommentOn= UrlToCommentOn,
                NumberOfPosts= NumberOfPosts,
                Width= Width,
                ColorScheme= ColorScheme
            };

            return View("~/Views/WeBlog/FacebookComments.cshtml", model);
        }
    }
}