using System;
using System.Web.UI;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.WebForms.WebControls
{
    public class Syndication : Sitecore.Web.UI.WebControl
    {
        protected ISyndicationInclude SyndicationLink;
        protected BaseLinkManager LinkManager;

        public Syndication(ISyndicationInclude sl, BaseLinkManager linkManager)
        {
            SyndicationLink = sl;

            if (SyndicationLink == null)
            {
                var feedResolver = ServiceLocator.ServiceProvider.GetRequiredService<IFeedResolver>();
                SyndicationLink = new SyndicationLink(feedResolver);
            }

            LinkManager = linkManager ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseLinkManager>();
        }

        [Obsolete("Use ctor(ISyndicationInclude, BaseLinkManager) instead.")]
        public Syndication(ISyndicationInclude sl = null)
            : this(sl, null)
        {
        }

        public Syndication()
            : this(null, null)
        {
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            if (SyndicationLink.ShouldInclude)
            {
                AddFeedToOutput(output);
            }
        }

        protected virtual void AddFeedToOutput(HtmlTextWriter output)
        {
            foreach (RssFeedItem feed in SyndicationLink.Feeds)
            {
                foreach (var a in SyndicationLink.Attributes)
                {
                    output.AddAttribute(a.Key, a.Value);
                }
                output.AddAttribute(HtmlTextWriterAttribute.Title, feed.Title.Raw);
                output.AddAttribute(HtmlTextWriterAttribute.Href, LinkManager.GetItemUrl(feed));
                output.RenderBeginTag(HtmlTextWriterTag.Link);
                output.RenderEndTag();
            }
        }
    }
}