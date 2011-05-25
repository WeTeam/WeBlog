using System;
using Sitecore.Links;
using Sitecore.Modules.Blog.Items.Blog;
using Sitecore.Modules.Blog.Managers;

namespace Sitecore.Modules.Blog.Layouts
{
    public partial class BlogRecentComments : BaseSublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var totalToShow = CurrentBlog.DisplayCommentSidebarCountNumeric;

            if (CommentManager.GetCommentsByBlog(CurrentBlog.ID, totalToShow).Length == 0)
            {
                PanelRecentComments.Visible = false;
            }
            else
            {
                ListViewRecentComments.DataSource = CommentManager.GetCommentsByBlog(CurrentBlog.ID, totalToShow);
                ListViewRecentComments.DataBind();

                titleRecentComments.Item = CurrentBlog.InnerItem;
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