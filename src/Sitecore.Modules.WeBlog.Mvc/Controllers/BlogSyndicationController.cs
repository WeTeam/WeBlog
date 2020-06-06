using System.Web.UI;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.Modules.WeBlog.Mvc.Controllers
{
    public class BlogSyndicationController : LinkIncludeController
    {
        protected ISyndicationInclude SyndicationLink { get; }

        protected BaseLinkManager LinkManager { get; }

        public BlogSyndicationController(ISyndicationInclude syndicationLink, BaseLinkManager linkManager)
            : base(syndicationLink)
        {
            Assert.ArgumentNotNull(syndicationLink, nameof(syndicationLink));
            Assert.ArgumentNotNull(linkManager, nameof(linkManager));

            SyndicationLink = syndicationLink;
            LinkManager = linkManager;
        }


        public BlogSyndicationController(ISyndicationInclude sl = null)
            : this(sl ?? new SyndicationLink(), ServiceLocator.ServiceProvider.GetRequiredService<BaseLinkManager>())
        {
        }

        protected override void AddLinkToOutput()
        {
            foreach (RssFeedItem feed in SyndicationLink.Feeds)
            {
                var include = new Tag(HtmlTextWriterTag.Link.ToString())
                {
                    Title = feed.Title.Raw,
                    Href = LinkManager.GetItemUrl(feed.InnerItem)
                };

                foreach (var a in SyndicationLink.Attributes)
                {
                    include.Attributes.Add(a.Key.ToString(), a.Value);
                }
                HttpContext.Response.Write(include.Start());
            }
        }
    }
}