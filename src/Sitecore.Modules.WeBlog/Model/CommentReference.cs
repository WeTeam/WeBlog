using System;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Model
{
    /// <summary>
    /// A reference to a comment.
    /// </summary>
    public class CommentReference
    {
        /// <summary>
        /// The Uri of the comment.
        /// </summary>
        public ItemUri Uri { get; set; }

        /// <summary>
        /// The Uri of the entry this comment is for.
        /// </summary>
        public ItemUri EntryUri { get; set; }

        /// <summary>
        /// The date the comment was created.
        /// </summary>
        public DateTime Created { get; set; }
    }
}