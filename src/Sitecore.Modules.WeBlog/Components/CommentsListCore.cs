using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Components
{
    public class CommentsListCore : ICommentsListCore
    {
        protected BlogHomeItem CurrentBlog { get; set; }

        protected EntryItem CurrentEntry { get; set; }

        protected IWeBlogSettings Settings { get; }

        protected ICommentManager CommentManager { get; }

        public CommentsListCore(BlogHomeItem currentBlogHomeItem, EntryItem currentEntry, IWeBlogSettings settings = null, ICommentManager commentManager = null)
        {
            CurrentBlog = currentBlogHomeItem;
            CurrentEntry = currentEntry;
            Settings = settings ?? WeBlogSettings.Instance;
            CommentManager = commentManager ?? ManagerFactory.CommentManagerInstance;
        }

        public virtual IList<CommentContent> LoadComments(CommentItem addedComment = null)
        {
            var comments = CommentManager.GetEntryComments(CurrentEntry, int.MaxValue).Select(x => CommentManager.GetCommentContent(x)).ToList();

            //if a comment has been added but is not coming back yet (i.e. being indexed), fake it
            if (addedComment != null && comments.All(comment => comment.Uri.ItemID != addedComment.ID))
            {
                CommentContent newComment = addedComment;
                comments.Insert(0, newComment);
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