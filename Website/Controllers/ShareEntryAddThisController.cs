using System.Web.Mvc;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class ShareEntryAddThisController : BlogBaseController
    {
        public ActionResult Index()
        {
            var addThisAccountName = Settings.AddThisAccountName;
            var model = string.IsNullOrEmpty(addThisAccountName) ? "" : "#pubid=" + addThisAccountName;

            return View("~/Views/WeBlog/ShareEntry-AddThis.cshtml", model);
        }
    }
}