using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Components.ThemeLink
{
    public interface IThemeInclude : ILinkInclude
    {
        ThemeItem CurrentTheme { get; }
    }
}