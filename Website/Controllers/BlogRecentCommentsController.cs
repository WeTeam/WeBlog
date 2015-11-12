using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogRecentCommentsController : BlogBaseController
    {
        protected IRecentCommentsCore RecentCommentsCore { get; set; }

        public BlogRecentCommentsController() : this(null) { }

        public BlogRecentCommentsController(IRecentCommentsCore recentCommentsCore)
        {
            RecentCommentsCore = recentCommentsCore ?? new RecentCommentsCore(ManagerFactory.BlogManagerInstance);
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
            return RecentCommentsCore.Comments.Select(commentItem => new RecentCommentsRenderingModel
            {
                CommentItem = commentItem,
                EntryTitle = RecentCommentsCore.GetEntryTitleForComment(commentItem),
                EntryUrl = RecentCommentsCore.GetEntryUrlForComment(commentItem)
            });
        }
    }
}