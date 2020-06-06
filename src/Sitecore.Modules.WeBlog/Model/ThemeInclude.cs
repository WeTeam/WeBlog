namespace Sitecore.Modules.WeBlog.Model
{
    /// <summary>
    /// A file to be included in a theme.
    /// </summary>
    public class ThemeInclude
    {
        /// <summary>
        /// Gets the URL of the file to include.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="url">The URL of the file to include.</param>
        public ThemeInclude(string url)
        {
            Url = url ?? string.Empty;
        }
    }
}
