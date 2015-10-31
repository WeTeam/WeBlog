using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Items.Feeds;

namespace Sitecore.Modules.WeBlog.Components.SyndicationLink
{
    public interface ISyndicationInclude : ILinkInclude
    {
        IEnumerable<RssFeedItem> Feeds { get; set; }
    }
}