using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml.Xsl;
using Sitecore.Web;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Data.Items;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Syndication;

namespace Sitecore.Modules.Eviblog.Webcontrols
{
    public class BlogMetaInfo : System.Web.UI.WebControls.WebControl
    {
        Blog currentBlog = BlogManager.GetCurrentBlog();

        protected override void Render(HtmlTextWriter writer)
        {
            // If Live Writer is enabled then add the rsd link
            if (currentBlog.EnableLiveWriter == true)
            {
                string rsdLink = "<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/EviBlog/rsd.ashx?blogid=" + currentBlog.ID + "\"/>" + Environment.NewLine;
                writer.Write(Environment.NewLine);
                writer.Write(rsdLink);
                writer.Write(Environment.NewLine);
            }

            // If RSS is enabled then add rss links
            if (currentBlog.EnableRSS == true)
            {
                Item currentBlogItem = BlogManager.GetCurrentBlogItem();
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
            string xmlRpc = "<link rel=\"pingback\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/EviBlog/MetaBlogApi.ashx?entryId=" + Sitecore.Context.Item.ID + "\" />" + Environment.NewLine;
            writer.Write(xmlRpc);
            writer.Write(Environment.NewLine);
        }

    }
}
