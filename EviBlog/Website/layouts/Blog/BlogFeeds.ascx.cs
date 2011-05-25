using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Modules.Blog.Items.Blog;
using Sitecore.Modules.Blog.Items.Feeds;
using Sitecore.Data.Items;
using System.Globalization;
using Sitecore.Modules.Blog.Layouts;

namespace Sitecore.Modules.Blog.Layouts
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