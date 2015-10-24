using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Components.CommentsList;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogCommentsListController : BlogBaseController
    {
        protected ICommentsListCore CommentsListCore { get; set; }

        public BlogCommentsListController() : this(null) { }

        public BlogCommentsListController(ICommentsListCore commentsListCore)
        {
            CommentsListCore = commentsListCore ?? new CommentsListCore(CurrentBlog);
        }

        public ActionResult Index()
        {
            if (CurrentEntry.DisableComments.Checked || ManagerFactory.CommentManagerInstance.GetCommentsCount() == 0)
            {
                return null;
            }
            var model = new CommentsListRenderingModel
            {
                Comments = CommentsListCore.LoadComments(),
                GetGravatarUrl = CommentsListCore.GetGravatarUrl,
                EnableGravatar = CurrentBlog.EnableGravatar.Checked,
                GravatarSizeNumeric = CurrentBlog.GravatarSizeNumeric,
                ShowEmailWithinComments = CurrentBlog.ShowEmailWithinComments.Checked
            };
            return View("~/Views/WeBlog/CommentsList.cshtml", model);
        }
    }
}