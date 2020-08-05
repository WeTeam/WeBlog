using Sitecore.Modules.WeBlog.Model;
using System.Collections.Specialized;

namespace Sitecore.Modules.WeBlog.Components
{
    /// <summary>
    /// Validates user submitted comments.
    /// </summary>
    public interface IValidateCommentCore
    {
        /// <summary>
        /// Validate a comment.
        /// </summary>
        /// <param name="comment">The comment to validate.</param>
        /// <param name="form">The form that was submitted.</param>
        /// <returns>The results of validation.</returns>
        CommentValidationResult Validate(Comment comment, NameValueCollection form);
    }
}
