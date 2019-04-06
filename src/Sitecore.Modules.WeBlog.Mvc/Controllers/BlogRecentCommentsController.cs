using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Mvc.Model;

namespace Sitecore.Modules.WeBlog.Mvc.Controllers
{
    public class BlogRecentCommentsController : BlogBaseController
    {
        protected IRecentCommentsCore RecentCommentsCore { get; set; }

        public BlogRecentCommentsController() : this(null) { }

        public BlogRecentCommentsController(IRecentCommentsCore recentCommentsCore)
        {
            if (recentCommentsCore != null)
                RecentCommentsCore = recentCommentsCore;
            else
            {
                RecentCommentsCore = new RecentCommentsCore();
                RecentCommentsCore.Initialise();
            }
        }

        public ActionResult Index()
        {
            if (RecentCommentsCore.Comments.Any())
            {
                var model = GetModel();
                return View("~/Views/WeBlog/RecentComments.cshtml", model);
            }
            return null;
        }

        protected virtual IEnumerable<RecentCommentsRenderingModel> GetModel()
        {
            return RecentCommentsCore.Comments.Select(entryComment => new RecentCommentsRenderingModel
            {
                Comment = entryComment.Comment,
                EntryTitle = entryComment.Entry.Title.Text,
                EntryUrl = LinkManager.GetItemUrl(entryComment.Entry)
            });
        }
    }
}