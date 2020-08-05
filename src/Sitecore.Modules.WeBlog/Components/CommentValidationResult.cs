using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Components
{
    /// <summary>
    /// Holds the results of validating a comment.
    /// </summary>
    public class CommentValidationResult
    {
        /// <summary>
        /// Gets any errors from the validation.
        /// </summary>
        public List<string> Errors { get; }

        /// <summary>
        /// Indicates whether the validation was a success or not.
        /// </summary>
        public bool Success => !Errors.Any();

        public CommentValidationResult(List<string> errors)
        {
            Errors = errors;
        }
    }
}
