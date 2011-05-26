using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Syndication;
using Sitecore.Links;

namespace Sitecore.Modules.Blog.Items.Feeds
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