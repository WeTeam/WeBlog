using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Components.PostList;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class PostList : BlogRenderingModelBase
    {
        protected const string DefaultPostTemplate = "~/Views/WeBlog/PostListEntry.cshtml";

        public IEnumerable<EntryItem> Entries { get; set; }
        public bool ShowViewMoreLink { get; set; }
        public string ViewMoreHref { get; set; }
        public string PostTemplate { get; set; }

        protected IPostListCore PostListCore { get; set; }

        public PostList() : this(null) { }

        public PostList(IPostListCore postListCore)
        {
            PostListCore = postListCore ?? new PostListCore(CurrentBlog);
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            var queryString = RenderingContext.Current.PageContext.RequestContext.HttpContext.Request.QueryString;
            PostListCore.Initialize(queryString);
            if (string.IsNullOrEmpty(PostTemplate))
            {
                PostTemplate = DefaultPostTemplate;
            }

            Entries = PostListCore.Entries;
            PostTemplate = PostTemplate;

            if (PostListCore.ShowViewMoreLink)
            {
                ShowViewMoreLink = true;
                ViewMoreHref = PostListCore.ViewMoreHref;
            }
            else
            {
                ShowViewMoreLink = false;
            }
        }
    }
}