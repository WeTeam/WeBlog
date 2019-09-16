using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Model
{
    /// <summary>
    /// A comment for a specific entry.
    /// </summary>
    public class EntryComment
    {
        /// <summary>
        /// Gets or sets the entry the comment is for.
        /// </summary>
        public EntryItem Entry { get; set; }

        /// <summary>
        /// Gets or sets the comment for the entry.
        /// </summary>
        public CommentContent Comment { get; set; }
    }
}