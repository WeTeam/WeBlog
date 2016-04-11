namespace Sitecore.Modules.WeBlog.Import
{
    public struct WpImportOptions
    {
        /// <summary>
        /// Determines if comments should be imported
        /// </summary>
        public bool IncludeComments { get; set; }
        /// <summary>
        /// Determines if categories should be imported
        /// </summary>
        public bool IncludeCategories { get; set; }
        /// <summary>
        /// Determines if tags should be imported
        /// </summary>
        public bool IncludeTags { get; set; }
    }
}