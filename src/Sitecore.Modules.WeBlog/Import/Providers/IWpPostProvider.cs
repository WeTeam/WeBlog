using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Import.Providers
{
    public interface IWpPostProvider
    {
        List<WpPost> GetPosts(WpImportOptions options);
    }
}