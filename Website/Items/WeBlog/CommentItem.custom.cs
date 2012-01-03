using System;
using Sitecore.Data.Items;
using Joel.Net;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public partial class CommentItem
    {
        /// <summary>
        /// Gets the creation date of the comment item.
        /// </summary>
        public DateTime Created
        {
            get
            {
                return InnerItem.Statistics.Created;
            }
        }

        /// <summary>
        /// For use with NVelocity token in email template
        /// </summary>
        public string AuthorName
        {
            get
            {
                return this.Name.Raw;
            }
        }

        /// <summary>
        /// Convert the comment to an AkismetComment for submission to Akismet
        /// </summary>
        /// <param name="comment">The comment to convert</param>
        /// <returns>An Akismet comment</returns>
        public static implicit operator AkismetComment(CommentItem comment)
        {
            var akismetComment = new AkismetComment();
            akismetComment.Blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog().Url;
            akismetComment.UserIp = comment.IPAddress.Text;
            akismetComment.UserAgent = ""; // TODO
            akismetComment.CommentContent = comment.Comment.Text;
            akismetComment.CommentType = "comment";
            akismetComment.CommentAuthor = comment.AuthorName;
            akismetComment.CommentAuthorEmail = comment.Email.Text;
            akismetComment.CommentAuthorUrl = comment.Website.Text;

            return akismetComment;
        }
    }
}