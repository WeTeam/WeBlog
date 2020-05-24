using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Links;

#if SC93
using Sitecore.Links.UrlBuilders;
#endif

namespace Sitecore.Modules.WeBlog.Extensions
{
    public static class BaseLinkManagerExtensions
    {
        public static string GetAbsoluteItemUrl(this BaseLinkManager linkManager, Item item)
        {
#if SC93
            var options = new ItemUrlBuilderOptions
#else
            var options = new UrlOptions
#endif
            {
                AlwaysIncludeServerUrl = true
            };

            return linkManager.GetItemUrl(item, options);
        }
    }
}
