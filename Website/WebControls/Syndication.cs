using System.Linq;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Items.Feeds;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class Syndication : Sitecore.Web.UI.WebControl
    {
        protected override void DoRender(HtmlTextWriter output)
        {
            var blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();

            if (blog != null && blog.EnableRss.Checked)
            {
                var feeds = blog.SyndicationFeeds;
                if (feeds != null && feeds.Count() > 0)
                {
                    foreach (RssFeedItem feed in feeds)
                    {
                        AddFeedToOutput(output, feed);
                    }
                }
            }
        }

        protected virtual void AddFeedToOutput(HtmlTextWriter output, RssFeedItem feed)
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