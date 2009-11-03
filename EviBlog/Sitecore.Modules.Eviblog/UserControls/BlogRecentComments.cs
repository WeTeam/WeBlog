using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogRecentComments : UserControl
    {
        #region Fields

        public Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        protected ListView ListViewRecentComments;
        protected Panel PanelRecentComments;
        protected Web.UI.WebControls.Text titleRecentComments;
        public int TotalToShow;

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            TotalToShow = currentBlog.DisplayCommentSidebarCount == 0 ? 0 : currentBlog.DisplayCommentSidebarCount;

            if(CommentManager.GetCommentsByBlog(currentBlog.ID, TotalToShow).Count == 0)
            {
                PanelRecentComments.Visible = false;
            }
            else
            {
                ListViewRecentComments.DataSource = CommentManager.GetCommentsByBlog(currentBlog.ID, TotalToShow);
                ListViewRecentComments.ItemDataBound += ListViewRecentComments_ItemDataBound;
                ListViewRecentComments.DataBind();

                titleRecentComments.Item = BlogManager.GetCurrentBlogItem();
            }
        }

        #endregion

        #region Eventhandlers

        private void ListViewRecentComments_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            Comment objComment = (Comment) ((ListViewDataItem) e.Item).DataItem;

            string textToDisplay = objComment.UserName + " wrote: " + objComment.CommentText;

            HyperLink commentLink = (HyperLink) e.Item.FindControl("hyperlinkComment");
            commentLink.NavigateUrl = LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(objComment.ID).Parent);
            commentLink.Text = textToDisplay;
        }

        #endregion
    }
}