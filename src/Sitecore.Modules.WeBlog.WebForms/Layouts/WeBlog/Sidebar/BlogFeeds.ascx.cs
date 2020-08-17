using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data;
using System;
using System.Linq;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar
{
    [AllowDependencyInjection]
    public partial class BlogFeeds : BaseSublayout
    {
        protected System.Web.UI.WebControls.PlaceHolder phFeeds;

        protected IFeedResolver FeedResolver { get; }

        public BlogFeeds(IFeedResolver feedResolver)
        {
            Assert.ArgumentNotNull(feedResolver, nameof(feedResolver));

            FeedResolver = feedResolver;
        }

        public BlogFeeds()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var feeds = FeedResolver.Resolve(CurrentBlog);
            if (feeds == null || feeds.Count() == 0)
            {
                this.Visible = false;
                return;
            }

            if (FeedList != null)
            {
                FeedList.DataSource = feeds;
                FeedList.DataBind();
            }
        }
    }
}