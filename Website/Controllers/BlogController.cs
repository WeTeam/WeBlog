using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogController : Controller
    {
        public ActionResult Index()
        {
            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            var model = new BlogRenderingModel
            {
                BlogItem = currentBlog,
                Hyperlink = currentBlog.SafeGet(x => x.Url)
            };
            return View("~/Views/WeBlog/Blog.cshtml", model);
        }
    }
}