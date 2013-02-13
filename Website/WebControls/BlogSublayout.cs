using System;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sites;
using Sitecore.Text;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class BlogSublayout : Sitecore.Web.UI.WebControls.Sublayout
    {
        public const string VARY_BY_BLOG = "varyByBlog";

        public override string GetCacheKey()
        {
            SiteContext site = Sitecore.Context.Site;
            if (!Cacheable || site == null || !site.CacheHtml || this.SkipCaching())
            {
                return string.Empty;
            }
            string key = base.GetCacheKey();
            UrlString parameters = new UrlString(this.Parameters);
            string varyByBlog = parameters[VARY_BY_BLOG];
            if (!string.IsNullOrEmpty(varyByBlog) && bool.TrueString.Equals(varyByBlog, StringComparison.OrdinalIgnoreCase))
            {
                var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
                key = key + "_#blog:" + currentBlog.SafeGet(x => x.ID).SafeGet(x => x.ToShortID()).SafeGet(x => x.ToString());
            }
            return key;
        }
    }
}