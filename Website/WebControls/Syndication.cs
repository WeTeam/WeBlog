using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Items.Feeds;
using System.Web.UI;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class Syndication : Sitecore.Web.UI.WebControl
    {
        protected override void DoRender(HtmlTextWriter output)
        {
            var blog = BlogManager.GetCurrentBlog();

            if (blog.EnableRSS.Checked)
            {
                var feeds = blog.SyndicationFeeds;
                if (feeds != null && feeds.Count() > 0)
                {
                    foreach (RSSFeedItem feed in feeds)
                    {
                        AddFeedToOutput(output, feed);
                    }
                }
            }
        }

        protected virtual void AddFeedToOutput(HtmlTextWriter output, RSSFeedItem feed)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Rel, "alternate");
            output.AddAttribute(HtmlTextWriterAttribute.Title, feed.Title.Text);
            output.AddAttribute(HtmlTextWriterAttribute.Type, "application/rss+xml");
            output.AddAttribute(HtmlTextWriterAttribute.Href, feed.Url);
            output.RenderBeginTag(HtmlTextWriterTag.Link);
            output.RenderEndTag();
        }
    }
}