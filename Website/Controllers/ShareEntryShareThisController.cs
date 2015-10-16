using System.Web.Mvc;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class ShareEntryShareThisController : BlogBaseController
    {
        public ActionResult Index()
        {
            return View("~/Views/WeBlog/ShareEntry-ShareThis.cshtml", Settings.ShareThisPublisherID);
        }
    }
}