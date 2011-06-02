using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Items.Blog;
using Sitecore.Modules.WeBlog.Items.Feeds;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Web;


namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class Blog : BaseSublayout
    {
        #region Page methods
        protected void Page_Load(object sender, EventArgs e)
        {
            fieldtextItem.DataSource = BlogManager.GetCurrentBlog().ID.ToString();
            HyperlinkBlog.NavigateUrl = LinkManager.GetItemUrl(BlogManager.GetCurrentBlog().InnerItem);

            Initialize();
        }

        /// <summary>
        /// Initializes this page.
        /// </summary>
        private void Initialize()
        {
            // If RSS is enabled then add the links to the page
            if (CurrentBlog.EnableRSS.Checked)
            {
                var feeds = CurrentBlog.SyndicationFeeds;
                if (feeds != null && feeds.Count() > 0)
                {
                    foreach (RSSFeedItem feed in feeds)
                    {
                        Literal rssLink = new Literal();
                        rssLink.Text = "<link rel=\"alternate\" title=" + feed.Title.Text + "  type=\"application/rss+xml\" href=\"" + feed.Url + "\" />" + Environment.NewLine;
                        Page.Header.Controls.Add(rssLink);
                    }
                }
            }

            // Set the correct theme file
            if (CurrentBlog.Theme.Raw != string.Empty)
            {
                string stylesheet = string.Empty;
                Item themeItem = CurrentBlog.Theme.Item;
                ThemeItem currentTheme = new ThemeItem(themeItem);

                Literal theme = new Literal();
                theme.Text = "<link href=\"" + currentTheme.FileLocation.Raw + "\" rel=\"stylesheet\" />" + Environment.NewLine;

                Page.Header.Controls.Add(theme);
            }

            // If Live Writer is enabled then add the rsd link
            if (CurrentBlog.EnableLiveWriter.Checked)
            {
                Literal rsdLink = new Literal();
                rsdLink.Text = "<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/Blog/rsd.ashx?blogid=" + CurrentBlog.ID + "\"/>" + Environment.NewLine;
                Page.Header.Controls.Add(rsdLink);
            }

            // Add the title to the page
            Page.Title = CurrentBlog.Title.Text;
        }
        #endregion
    }
}