using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Data
{
    public class FeedResolver : IFeedResolver
    {
        protected BaseTemplateManager TemplateManager { get; }

        protected IWeBlogSettings WeBlogSettings { get; }

        public FeedResolver(BaseTemplateManager templateManager, IWeBlogSettings weBlogSettings)
        {
            Assert.ArgumentNotNull(templateManager, nameof(templateManager));
            Assert.ArgumentNotNull(weBlogSettings, nameof(weBlogSettings));

            TemplateManager = templateManager;
            WeBlogSettings = weBlogSettings;
        }

        public IEnumerable<RssFeedItem> Resolve(BlogHomeItem blogItem)
        {
            if(blogItem == null)
                yield break;

            if (blogItem.EnableRss.Checked)
            {
                var children = blogItem.InnerItem.GetChildren();
                if (children == null)
                    yield break;

                foreach (Item child in children)
                {
                    if(TemplateManager.TemplateIsOrBasedOn(child, WeBlogSettings.RssFeedTemplateIds))
                        yield return new RssFeedItem(child);
                }
            }
        }
    }
}
