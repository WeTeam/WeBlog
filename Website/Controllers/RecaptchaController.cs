using System.Web.Mvc;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class RecaptchaController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/WeBlog/Recaptcha.cshtml"); 
        }
    }
}