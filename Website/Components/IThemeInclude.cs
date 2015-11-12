using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface IThemeInclude : ILinkInclude
    {
        ThemeItem CurrentTheme { get; }
    }
}