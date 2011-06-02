using System;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Layouts
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
            FeedList.DataSource = feeds;
            FeedList.DataBind();
        }
    }
}