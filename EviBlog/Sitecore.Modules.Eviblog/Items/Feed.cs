using Sitecore.Data.Items;
using Sitecore.Syndication;
using Sitecore.Links;

namespace Sitecore.Modules.Eviblog.Items
{
    public class Feed : CustomItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Feed"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public Feed(Item item): base(item)
        {
        }

        public string Title
        {
            get
            {
                return InnerItem["Title"];
            }
        }

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
