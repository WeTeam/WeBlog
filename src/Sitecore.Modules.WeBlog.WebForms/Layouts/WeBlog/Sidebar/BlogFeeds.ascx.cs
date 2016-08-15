using System;
using System.Linq;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog.Sidebar
{
    public partial class BlogFeeds : BaseSublayout
    {
        protected System.Web.UI.WebControls.PlaceHolder phFeeds;
            
        protected void Page_Load(object sender, EventArgs e)
        {
            var feeds = CurrentBlog.SyndicationFeeds;
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