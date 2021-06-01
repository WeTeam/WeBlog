using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Model;
using System.Runtime.Serialization;

namespace Sitecore.Modules.WeBlog.Model
{
    /// <summary>
    /// An event raised when comments are submitted.
    /// </summary>
    [DataContract]
    public class CommentSubmitted
    {
        /// <summary>
        /// Gets or sets the ID of the entry the comment was submitted for.
        /// </summary>
        [DataMember]
        public ID EntryId { get; set; }

        /// <summary>
        /// Gets or sets the language of the entry the comment was submitted for.
        /// </summary>
        [DataMember]
        public Language Language { get; set; }

        /// <summary>
        /// Gets or sets the comment that was submitted.
        /// </summary>
        [DataMember]
        public Comment Comment { get; set; }

        /// <summary>
        /// Gets or sets the ID requested to be used for the newly created comment.
        /// </summary>
        [DataMember]
        public ID RequestedCommentId { get; set; }
    }
}
