using System;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.Mvc.Pipelines.RenderRendering
{
    public class AppendVaryByBlogParameter : RenderRenderingProcessor
    {
        public override void Process(RenderRenderingArgs args)
        {
            if (args.CacheKey != null)
            {
                if (args.CacheKey.Contains("VaryByBlog"))
                {
                    UpdateParametersForCaching(args);
                }
            }
        }

        public void UpdateParametersForCaching(RenderRenderingArgs args)
        {
            SiteContext site = Context.Site;
            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            if (args.Rendering.Caching.Cacheable && site != null && site.CacheHtml && currentBlog != null)
            {
                var key = "CacheVaryByBlogKey=" + currentBlog?.ID?.ToShortID()?.ToString();
                args.CacheKey +=  String.Format("&{0}", key);
            }
        }
    }
}
