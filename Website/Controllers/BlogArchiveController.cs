using System.ComponentModel;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Components.Archive;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogArchiveController : BlogBaseController
    {
        protected IArchiveCore ArchiveCore { get; set; }

        public BlogArchiveController() : this(null) { }

        public BlogArchiveController(IArchiveCore archiveCore)
        {
            ArchiveCore = archiveCore ?? new ArchiveCore(ManagerFactory.BlogManagerInstance);
        }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ExpandMonthsOnLoad { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ExpandPostsOnLoad { get; set; }

        public ActionResult Index()
        {
            var model = new ArchiveRenderingModel
            {
                ExpandMonthsOnLoad = ExpandMonthsOnLoad,
                ExpandPostsOnLoad = ExpandPostsOnLoad,
                ArchiveCore = ArchiveCore
            };
            return View("~/Views/WeBlog/Archive.cshtml", model);
        }
    }
}