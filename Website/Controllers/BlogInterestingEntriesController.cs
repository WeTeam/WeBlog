using System.Linq;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogInterestingEntriesController : BlogBaseController
    {
        protected IInterstingEntriesAlgorithm InterestingEntriesCore { get; set; }

        public string Mode { get; set; }

        public int MaximumCount { get; set; }

        public BlogInterestingEntriesController() : this(null) { }

        public BlogInterestingEntriesController(IInterstingEntriesAlgorithm interestingEntriesCore)
        {
            InterestingEntriesCore = interestingEntriesCore;
            var algororithm = Components.InterestingEntriesCore.GetAlgororithmFromString(Mode);
            if (algororithm != InterestingEntriesAlgorithm.Custom || InterestingEntriesCore == null)
            {
                InterestingEntriesCore = new InterestingEntriesCore(ManagerFactory.EntryManagerInstance, algororithm);
            }
        }

        public ActionResult Index()
        {
            var entries = InterestingEntriesCore.GetEntries(CurrentBlog, MaximumCount);
            if (entries.Any())
            {
                return View("~/Views/WeBlog/InterestingEntries.cshtml", entries);
            }
            return null;
        }
    }
}