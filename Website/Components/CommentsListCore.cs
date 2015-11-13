using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Components
{
    public class CommentsListCore : ICommentsListCore
    {
        protected BlogHomeItem CurrentBlog { get; set; }

        public CommentsListCore(BlogHomeItem currentBlogHomeItem)
        {
            CurrentBlog = currentBlogHomeItem;
        }

        public virtual CommentItem[] LoadComments(CommentItem addedComment = null)
        {
            CommentItem[] comments = ManagerFactory.CommentManagerInstance.GetEntryComments();
            //if a comment has been added but is not coming back yet (i.e. being indexed), fake it
            if (addedComment != null && comments.Count(comment => comment.ID == addedComment.ID) == 0)
            {
                List<CommentItem> newList = new List<CommentItem>
                {
                    addedComment
                };
                newList.AddRange(comments);
                comments = newList.ToArray();
            }
            return comments;
        }

        /// <summary>
        /// Get the URL for the user's gravatar image
        /// </summary>
        /// <param name="email">The email address of the user to get the gravatar for</param>
        /// <returns>The URL for the gravatar image</returns>
        public string GetGravatarUrl(string email)
        {
            var baseUrl = Settings.GravatarImageServiceUrl;
            if (!string.IsNullOrEmpty(baseUrl))
            {
                return baseUrl + "/" + Crypto.GetMD5Hash(email) + ".jpg" +
                       "?s=" + CurrentBlog.GravatarSizeNumeric +
                       "&d=" + CurrentBlog.DefaultGravatarStyle.Raw +
                       "&r=" + CurrentBlog.GravatarRating.Raw;
            }
            return string.Empty;
        }
    }
}