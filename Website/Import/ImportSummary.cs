using System;

namespace Sitecore.Modules.WeBlog.Import
{
    /// <summary>
    /// Contains summary results of an import process.
    /// </summary>
    [Serializable]
    public class ImportSummary
    {
        /// <summary>
        /// Gets or sets the number of posts imported.
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// Gets or sets the number of categories imported.
        /// </summary>
        public int CategoryCount { get; set; }

        /// <summary>
        /// Gets or sets the number of comments imported.
        /// </summary>
        public int CommentCount { get; set; }
    }
}