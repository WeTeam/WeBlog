using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Items.Feeds;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Components
{
    public class SyndicationLink : ISyndicationInclude
    {
        protected BlogHomeItem Blog;
        public Dictionary<HtmlTextWriterAttribute, string> Attributes { get; set; }
        public IEnumerable<RSSFeedItem> Feeds { get; set; }

        public virtual bool ShouldInclude
        {
            get
            {
                if (Blog != null && Blog.EnableRSS.Checked)
                {
                    var feeds = Blog.SyndicationFeeds;
                    return feeds != null && feeds.Any();
                }
                return false;
            }
        }

        public SyndicationLink()
        {
            Blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            Feeds = Blog.SyndicationFeeds;
            Attributes = new Dictionary<HtmlTextWriterAttribute, string>
            {
                {HtmlTextWriterAttribute.Rel, "alternate"},
                {HtmlTextWriterAttribute.Type, "application/rsd+xml"},
            };
        }
    }
}