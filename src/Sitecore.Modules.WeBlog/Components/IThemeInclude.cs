using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface IThemeInclude : ILinkInclude
    {
        ThemeItem CurrentTheme { get; }
    }
}