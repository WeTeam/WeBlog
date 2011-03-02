using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Data.Items;
using System.Globalization;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public partial class BlogFeeds : System.Web.UI.UserControl
    {
        protected System.Web.UI.WebControls.PlaceHolder phFeeds;
            
        protected void Page_Load(object sender, EventArgs e)
        {
            Sitecore.Modules.Eviblog.Items.Blog blog = BlogManager.GetCurrentBlog();
            List<Feed> feeds = BlogFeedManager.GetBlogFeeds(blog.ID);
            if (!blog.EnableRSS || feeds == null || feeds.Count == 0)
            {
                this.Visible = false;
                return;
            }

            foreach (Feed feed in feeds)
            {
                HyperLink imageLink = new HyperLink();
                imageLink.NavigateUrl = feed.Url;
                imageLink.ImageUrl = "/sitecore modules/EviBlog/Images/feed-icon-14x14.png";
                imageLink.CssClass = "feedImage";
                HyperLink textLink = new HyperLink();
                textLink.NavigateUrl = feed.Url;
                textLink.Text = feed.Title;
                textLink.CssClass = "feedText";
                AddListItem(imageLink, textLink);
            }

        }

        private void AddListItem(Control image, Control link)
        {
            phFeeds.Controls.Add(new LiteralControl("<li>"));
            phFeeds.Controls.Add(image);
            phFeeds.Controls.Add(link);
            phFeeds.Controls.Add(new LiteralControl("</li>"));
        }
    }
}