using System.Web.UI;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Components;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class Syndication : Sitecore.Web.UI.WebControl
    {
        protected ISyndicationInclude SyndicationLink;

        public Syndication(ISyndicationInclude sl = null)
        {
            SyndicationLink = sl ?? new SyndicationLink();
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
                output.AddAttribute(HtmlTextWriterAttribute.Href, feed.Url);
                output.RenderBeginTag(HtmlTextWriterTag.Link);
                output.RenderEndTag();
            }
        }
    }
}