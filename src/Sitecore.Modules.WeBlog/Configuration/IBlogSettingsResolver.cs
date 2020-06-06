using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Configuration
{
    public interface IBlogSettingsResolver
    {
        BlogSettings Resolve(BlogHomeItem blogItem);
    }
}