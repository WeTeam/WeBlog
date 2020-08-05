using Sitecore.Modules.WeBlog.Model;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Sitecore.Modules.WeBlog.Pipelines.ValidateComment
{
    /// <summary>
    /// Arguments for the weblogValidateComment pipeline.
    /// </summary>
    public class ValidateCommentArgs : PipelineArgs
    {
        /// <summary>
        /// Gets the comment being validated.
        /// </summary>
        public Comment Comment { get; }

        /// <summary>
        /// Gets the form fields submiited for the comment.
        /// </summary>
        public NameValueCollection Form { get; }

        /// <summary>
        /// Gets the list of errors for the comment.
        /// </summary>
        public List<string> Errors { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="comment">The comment being validated.</param>
        /// <param name="form">The form fields submiited for the comment.</param>
        public ValidateCommentArgs(Comment comment, NameValueCollection form)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            Comment = comment;
            Form = form;
            Errors = new List<string>();
        }
    }
}
