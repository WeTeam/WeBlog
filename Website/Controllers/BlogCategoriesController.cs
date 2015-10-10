using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogCategoriesController : Controller
    {
        public ActionResult Index()
        {
            var categoryItems = ManagerFactory.CategoryManagerInstance.GetCategories();
            return View("~/Views/WeBlog/Categories.cshtml", categoryItems);
        }
    }
}