using Sitecore.Links;
using Sitecore.Syndication;

namespace Sitecore.Modules.WeBlog.Items.Feeds
{
    public partial class RSSFeedItem
    {
        public string Url
        {
            get
            {
                PublicFeed feed = FeedManager.GetFeed(InnerItem);
                return feed.GetUrl(UrlOptions.DefaultOptions, false);
            }
        }
    }
}