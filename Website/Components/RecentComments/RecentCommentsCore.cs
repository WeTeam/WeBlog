using Sitecore.Links;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Components.RecentComments
{
    public class RecentCommentsCore : IRecentCommentsCore
    {
        protected BlogHomeItem CurrentBlog { get; set; }
        public CommentItem[] Comments { get; set; }

        public RecentCommentsCore(IBlogManager blogManager)
        {
            CurrentBlog = blogManager.GetCurrentBlog();
            var totalToShow = CurrentBlog.DisplayCommentSidebarCountNumeric;
            Comments = ManagerFactory.CommentManagerInstance.GetCommentsByBlog(CurrentBlog.ID, totalToShow);
        }

        /// <summary>
        /// Get the URL of the blog entry a comment was made against
        /// </summary>
        /// <param name="comment">The comment to find the blog entry URL for</param>
        /// <returns>The URL if found, otherwise an empty string</returns>
        public virtual string GetEntryUrlForComment(CommentItem comment)
        {
            if (comment != null)
                return LinkManager.GetItemUrl(ManagerFactory.EntryManagerInstance.GetBlogEntryByComment(comment).InnerItem);
            else
                return string.Empty;
        }

        /// <summary>
        /// Get the name of the blog entry a comment was made against
        /// </summary>
        /// <param name="comment">The comment to find the blog entry title for</param>
        /// <returns>The title if found, otherwise an empty string</returns>
        public virtual string GetEntryTitleForComment(CommentItem comment)
        {
            if (comment != null)
                return ManagerFactory.EntryManagerInstance.GetBlogEntryByComment(comment).Title.Text;
            else
                return string.Empty;
        }
    }
}