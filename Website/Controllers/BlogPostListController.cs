using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Components.PostList;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogPostListController : BlogBaseController
    {
        protected const string DefaultPostTemplate = "/Views/WeBlog/PostListEntry.cshtml";

        protected IPostListCore PostListCore { get; set; }

        public BlogPostListController() : this(null) { }

        public BlogPostListController(IPostListCore postListCore)
        {
            PostListCore = postListCore ?? new PostListCore(CurrentBlog);
        }

        /// <summary>
        /// Gets or sets the path to the (override) template for posts in the list.
        /// </summary>
        public string PostTemplate { get; set; }

        public ActionResult Index()
        {
            PostListCore.Initialize(Request.QueryString);
            if (string.IsNullOrEmpty(PostTemplate))
            {
                PostTemplate = DefaultPostTemplate;
            }

            var model = new PostListRenderingModel
            {
                Entries = PostListCore.Entries,
                PostTemplate = PostTemplate
            };

            if (PostListCore.ShowViewMoreLink)
            {
                model.ShowViewMoreLink = true;
                model.ViewMoreHref = PostListCore.ViewMoreHref;
            }
            else
            {
                model.ShowViewMoreLink = false;
            }

            return View("~/Views/WeBlog/PostList.cshtml", model);
        }
    }
}