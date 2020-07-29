using Sitecore.Modules.WeBlog.Data;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Sitecore.Modules.WeBlog.Components
{
    public class SyndicationLink : ISyndicationInclude
    {
        protected BlogHomeItem Blog;
        public Dictionary<HtmlTextWriterAttribute, string> Attributes { get; set; }
        public IEnumerable<RssFeedItem> Feeds { get; set; }

        public virtual bool ShouldInclude
        {
            get
            {
                return Feeds.Any();
            }
        }

        public SyndicationLink(IFeedResolver feedResolver)
        {
            Blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            Feeds = feedResolver.Resolve(Blog);
            Attributes = new Dictionary<HtmlTextWriterAttribute, string>
            {
                {HtmlTextWriterAttribute.Rel, "alternate"},
                {HtmlTextWriterAttribute.Type, "application/rsd+xml"},
            };
        }
    }
}