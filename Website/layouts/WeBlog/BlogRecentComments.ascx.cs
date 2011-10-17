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

            if (CommentManager.GetCommentsByBlog(CurrentBlog.ID, totalToShow).Length == 0)
            {
                if(PanelRecentComments != null)
                    PanelRecentComments.Visible = false;
            }
            else
            {
                if (ListViewRecentComments != null)
                {
                    ListViewRecentComments.DataSource = CommentManager.GetCommentsByBlog(CurrentBlog.ID, totalToShow);
                    ListViewRecentComments.DataBind();
                }
            }
        }

        /// <summary>
        /// Get the URL of the blog a comment was made against
        /// </summary>
        /// <param name="comment">The comment to find the blog URL for</param>
        /// <returns>The URL if found, otherwise an empty string</returns>
        protected virtual string GetEntryUrlForComment(CommentItem comment)
        {
            if (comment != null)
                return LinkManager.GetItemUrl(comment.InnerItem.Parent);
            else
                return string.Empty;
        }
    }
}