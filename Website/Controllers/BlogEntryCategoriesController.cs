using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogEntryCategoriesController : BlogBaseController
    {
        public ActionResult Index()
        {
            var categoryItems = ManagerFactory.CategoryManagerInstance.GetCategoriesByEntryID(CurrentEntry.ID);
            return View("~/Views/WeBlog/EntryCategories.cshtml", categoryItems);
        }
    }
}