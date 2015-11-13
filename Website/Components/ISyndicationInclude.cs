using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface ISyndicationInclude : ILinkInclude
    {
        IEnumerable<RssFeedItem> Feeds { get; set; }
    }
}