using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.Eviblog.Items;

namespace Sitecore.Modules.Eviblog.Managers
{
    public class BlogFeedManager
    {
        public static List<Feed> GetBlogFeeds(ID BlogID)
        {
            List<Feed> feeds = null;
            Blog blog = new Blog(BlogManager.GetBlogByID(BlogID));
            if (blog.EnableRSS)
            {
                Item[] feedItems = blog.InnerItem.Axes.SelectItems("./*[@@templatename='RSS Feed']");
                if (feedItems != null)
                {
                    feeds = new List<Feed>();
                    foreach (Item item in feedItems)
                    {
                        feeds.Add(new Feed(item));
                    }
                }
            }
            return feeds;
        }
    }
}
