using System;
using Sitecore.Pipelines;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Data;
using Sitecore.Globalization;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class CreateCommentArgs : PipelineArgs
    {
        /// <summary>
        /// Gets or sets the ID of the blog entry the comment is being created against
        /// </summary>
        public ID EntryID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comment data
        /// </summary>
        public Comment Comment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comment item that was created
        /// </summary>
        public CommentItem CommentItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the database to create the comment in
        /// </summary>
        public Database Database
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the language to create the comment in
        /// </summary>
        public Language Language
        {
            get;
            set;
        }
    }
}