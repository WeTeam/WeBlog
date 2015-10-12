using System.Web.Mvc;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogFeedsController : BlogBaseController
    {
        public ActionResult Index()
        {
            var feeds = CurrentBlog.SyndicationFeeds;
            if (feeds != null)
            {
                return View("~/Views/WeBlog/Feeds.cshtml", feeds);
            }
            return null;
        }
    }
}