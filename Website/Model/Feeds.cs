using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Items.Feeds;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class Feeds : BlogRenderingModelBase
    {
        public IEnumerable<RssFeedItem> FeedItems { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            FeedItems = CurrentBlog.SyndicationFeeds;
        }
    }
}