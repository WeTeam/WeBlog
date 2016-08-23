using System.Web.UI;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.Modules.WeBlog.Mvc.Controllers
{
    public class BlogSyndicationController : LinkIncludeController
    {
        protected ISyndicationInclude SyndicationLink;

        public BlogSyndicationController() : this(new SyndicationLink()) { }

        public BlogSyndicationController(ISyndicationInclude sl = null) : base(sl)
        {
            SyndicationLink = sl ?? new SyndicationLink();
        }

        protected override void AddLinkToOutput()
        {
            foreach (RssFeedItem feed in SyndicationLink.Feeds)
            {
                var include = new Tag(HtmlTextWriterTag.Link.ToString())
                {
                    Title = feed.Title.Raw,
                    Href = feed.Url
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