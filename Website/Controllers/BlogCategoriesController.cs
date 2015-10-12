using System.Linq;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogCategoriesController : BlogBaseController
    {
        public ActionResult Index()
        {
            var categoryItems = ManagerFactory.CategoryManagerInstance.GetCategories();
            if (categoryItems.Any())
            {
                return View("~/Views/WeBlog/Categories.cshtml", categoryItems);
            }
            return null;
        }
    }
}