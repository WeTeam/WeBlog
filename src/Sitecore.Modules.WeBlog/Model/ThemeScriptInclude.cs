namespace Sitecore.Modules.WeBlog.Model
{
    /// <summary>
    /// A script to be included in a theme.
    /// </summary>
    public class ThemeScriptInclude : ThemeInclude
    {
        /// <summary>
        /// Gets the URL of a fallback source for the script.
        /// </summary>
        public string FallbackUrl { get; }

        /// <summary>
        /// Gets the name of the Javascript object which the script populates into the global namespace which can be used to validate if the script has been loaded.
        /// </summary>
        public string VerificationObject { get;  }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="url">The URL of the file to include.</param>
        /// <param name="fallbackUrl">The URL of a fallback source for the script.</param>
        /// <param name="verificationObject">The name of the Javascript object which the script populates into the global namespace which can be used to validate if the script has been loaded.</param>
        public ThemeScriptInclude(string url, string fallbackUrl, string verificationObject)
            : base(url)
        {
            FallbackUrl = fallbackUrl ?? string.Empty;
            VerificationObject = verificationObject ?? string.Empty;
        }
    }
}
