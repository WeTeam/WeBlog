using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Themes
{
    /// <summary>
    /// Resolves the file references for a theme.
    /// </summary>
    public interface IThemeFileResolver
    {
        /// <summary>
        /// Resolve the file references for the given theme.
        /// </summary>
        /// <param name="themeItem">The theme item to resolve the file references for.</param>
        /// <returns>The file references for the theme.</returns>
        ThemeFiles Resolve(ThemeItem themeItem);
    }
}
