using System;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Syndication;
using Sitecore.Web;

namespace Sitecore.Modules.Blog.WebControls
{
    public class BlogMetaInfo : System.Web.UI.WebControls.WebControl
    {
        Sitecore.Modules.Blog.Items.Blog.BlogItem currentBlog = BlogManager.GetCurrentBlog();

        protected override void Render(HtmlTextWriter writer)
        {
            // If Live Writer is enabled then add the rsd link
            if (currentBlog.EnableLiveWriter.Checked)
            {
                string rsdLink = "<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/Blog/rsd.ashx?blogid=" + currentBlog.ID + "\"/>" + Environment.NewLine;
                writer.Write(Environment.NewLine);
                writer.Write(rsdLink);
                writer.Write(Environment.NewLine);
            }

            // If RSS is enabled then add rss links
            if (currentBlog.EnableRSS.Checked)
            {
                Item currentBlogItem = BlogManager.GetCurrentBlog().InnerItem;
                Item[] feedItem = currentBlogItem.Axes.SelectItems("./*[@@templatename='RSS Feed']");
                
                foreach (Item item in feedItem)
                {
                    PublicFeed feed = FeedManager.GetFeed(item);
                    
                    string rsslink = "<link rel=\"alternate\" title=" + feed.FeedItem.Name + "  type=\"application/rss+xml\" href=\"" + FeedManager.GetFeedUrl(item, false) + "\" />" + Environment.NewLine;
                    writer.Write(rsslink);
                    writer.Write(Environment.NewLine);
                }
            }

            // Add the xml-rpc link
            string xmlRpc = "<link rel=\"pingback\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/Blog/MetaBlogApi.ashx?entryId=" + Sitecore.Context.Item.ID + "\" />" + Environment.NewLine;
            writer.Write(xmlRpc);
            writer.Write(Environment.NewLine);
        }

    }
}
