using System;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Managers;
using System.Web.UI.WebControls;
using Sitecore.Web;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Modules.Eviblog.Items;
using System.Web.UI;
using Sitecore.Syndication;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public partial class Blog : System.Web.UI.UserControl
    {
        #region Fields
        protected HyperLink HyperlinkBlog;
        protected Sitecore.Web.UI.WebControls.Text fieldtextItem;
        public Sitecore.Modules.Eviblog.Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        public Sitecore.Modules.Eviblog.Items.Settings currentblogSettings = BlogManager.GetCurrentBlogSettings();
        #endregion

        #region Page methods
        protected void Page_Load(object sender, EventArgs e)
        {
            this.fieldtextItem.DataSource = BlogManager.GetCurrentBlogID().ToString();
            this.HyperlinkBlog.NavigateUrl = LinkManager.GetItemUrl(BlogManager.GetBlogByID(BlogManager.GetCurrentBlogID()));

            Initialize();
        }
        
        /// <summary>
        /// Initializes this page.
        /// </summary>
        private void Initialize()
        {
            // Get the placeholders
            PlaceHolder phEviblog = (PlaceHolder)Page.FindControl("phEviblog");
            PlaceHolder phEviblogTitle = (PlaceHolder)Page.FindControl("phEviblogTitle");

            // If RSS is enabled then add the links to the page
            if (phEviblog != null && currentBlog.EnableRSS == true)
            {
                Item currentBlogItem = BlogManager.GetCurrentBlogItem();
                Item[] feedItem = currentBlogItem.Axes.SelectItems("./*[@@templatename='RSS Feed']");

                foreach (Item item in feedItem)
                {
                    PublicFeed feed = FeedManager.GetFeed(item);

                    Literal rssLink = new Literal();
                    rssLink.Text = "<link rel=\"alternate\" title=" + feed.FeedItem.Name + "  type=\"application/rss+xml\" href=\"" + FeedManager.GetFeedUrl(item, false) + "\" />" + Environment.NewLine;
                    phEviblog.Controls.Add(rssLink);
                }

                
            }

            // Set the correct theme file
            if (phEviblog != null && currentBlog.Theme != string.Empty)
            {
                string stylesheet = string.Empty;

                Item themeItem = Sitecore.Context.Database.GetItem(new ID(currentBlog.Theme));
                Theme currentTheme = new Theme(themeItem);

                Literal theme = new Literal();
                theme.Text = "<link href=\"" + currentTheme.File + "\" rel=\"stylesheet\" />" + Environment.NewLine;

                phEviblog.Controls.Add(theme);
            }

            // If Live Writer is enabled then add the rsd link
            if (phEviblog != null && currentBlog.EnableLiveWriter == true)
            {
                Literal rsdLink = new Literal();
                rsdLink.Text = "<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/EviBlog/rsd.ashx?blogid=" + currentBlog.ID + "\"/>" + Environment.NewLine;
                phEviblog.Controls.Add(rsdLink);
            }
            
            // Add the title to the page
            if (phEviblogTitle != null)
            {
                phEviblogTitle.Controls.Add(new LiteralControl(currentBlog.Name));
            }
        }
        #endregion
    }
}