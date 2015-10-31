using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogTwitterController : BlogBaseController
    {
        public string Username { get; set; }
        public string WidgetId { get; set; }
        public int NumberOfTweets { get; set; }
        public string Theme { get; set; }
        public string BorderColour { get; set; }
        public string LinkColour { get; set; }
        public string Chrome { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Width))
            {
                Width = "auto";
            }

            if (string.IsNullOrEmpty(Height))
            {
                Height = "auto";
            }

            var model = new TwitterTimelineRenderingModel
            {
                Username = Username,
                WidgetId = WidgetId,
                TweetLimit = NumberOfTweets,
                Theme = Theme,
                BorderColor = BorderColour,
                LinkColor = LinkColour,
                Chrome = Chrome,
                Width = Width,
                Height = Height,

            };
            return View("~/Views/WeBlog/TwitterTimeline.cshtml", model);
        }
    }
}