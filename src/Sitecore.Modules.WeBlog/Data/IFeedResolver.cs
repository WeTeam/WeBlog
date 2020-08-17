using Sitecore.Modules.WeBlog.Data.Items;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Data
{
    /// <summary>
    /// Defines a service which resolves Syndication feeds.
    /// </summary>
    public interface IFeedResolver
    {
        /// <summary>
        /// Resolves the syndication feeds for the provided <see cref="BlogHomeItem"/>.
        /// </summary>
        /// <param name="blogItem">The blog item to resolve the feeds for.</param>
        /// <returns>The feed items for the blog item.</returns>
        IEnumerable<RssFeedItem> Resolve(BlogHomeItem blogItem);
    }
}
