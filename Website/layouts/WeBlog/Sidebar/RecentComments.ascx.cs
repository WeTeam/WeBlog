using System;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogRecentComments : BaseSublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var totalToShow = CurrentBlog.DisplayCommentSidebarCountNumeric;

            var comments = ManagerFactory.CommentManagerInstance.GetCommentsByBlog(CurrentBlog.ID, totalToShow);
            if (comments.Length == 0)
            {
                if(PanelRecentComments != null)
                    PanelRecentComments.Visible = false;
            }
            else
            {
                if (ListViewRecentComments != null)
                {
                    ListViewRecentComments.DataSource = comments;
                    ListViewRecentComments.DataBind();
                }
            }
        }

        /// <summary>
        /// Get the URL of the blog entry a comment was made against
        /// </summary>
        /// <param name="comment">The comment to find the blog entry URL for</param>
        /// <returns>The URL if found, otherwise an empty string</returns>
        protected virtual string GetEntryUrlForComment(CommentItem comment) 
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
        protected virtual string GetEntryTitleForComment(CommentItem comment)
        {
            if (comment != null)
                return ManagerFactory.EntryManagerInstance.GetBlogEntryByComment(comment).Title.Text;
            else
                return string.Empty;
        }
    }
}