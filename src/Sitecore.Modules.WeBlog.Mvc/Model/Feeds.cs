using System.Collections.Generic;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class Feeds : BlogRenderingModelBase
    {
        protected IFeedResolver FeedResolver { get; }

        public IEnumerable<RssFeedItem> FeedItems { get; set; }

        public Feeds(IFeedResolver feedResolver)
        {
            Assert.ArgumentNotNull(feedResolver, nameof(feedResolver));

            FeedResolver = feedResolver;
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            FeedItems = FeedResolver.Resolve(CurrentBlog);
        }
    }
}