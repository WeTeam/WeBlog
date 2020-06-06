using Sitecore.Modules.WeBlog.Data.Items;
using System;

namespace Sitecore.Modules.WeBlog.Components
{
    [Obsolete("Use Sitecore.Modules.WeBlog.Themes.IThemeFileResolver from services instead.")]
    public interface IThemeInclude : ILinkInclude
    {
        ThemeItem CurrentTheme { get; }
    }
}