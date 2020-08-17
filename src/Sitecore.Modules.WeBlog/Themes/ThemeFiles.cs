using Sitecore.Modules.WeBlog.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Themes
{
    /// <summary>
    /// The collection of file includes for a theme.
    /// </summary>
    public class ThemeFiles
    {
        /// <summary>
        /// Gets the stylesheet includes for the theme.
        /// </summary>
        public IEnumerable<ThemeInclude> Stylesheets { get; }

        /// <summary>
        /// Gets the script includes for the theme.
        /// </summary>
        public IEnumerable<ThemeScriptInclude> Scripts { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="stylesheets">The stylesheet items for the theme.</param>
        /// <param name="scripts">The script items for the theme.</param>
        public ThemeFiles(IEnumerable<ThemeInclude> stylesheets, IEnumerable<ThemeScriptInclude> scripts)
        {
            Stylesheets = stylesheets ?? Enumerable.Empty<ThemeInclude>();
            Scripts = scripts ?? Enumerable.Empty<ThemeScriptInclude>();
        }
    }
}
