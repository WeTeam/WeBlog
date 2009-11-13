using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Modules.Eviblog.Utilities;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogPostList : UserControl
    {
        #region Fields

        public Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        protected ListView ListView1;
        public int TotalToShow;

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            TotalToShow = currentBlog.DisplayItemCount == 0 ? 0 : currentBlog.DisplayItemCount;

            if (!Page.IsPostBack && Request.QueryString["tag"].HasValue())
            {
                ListView1.DataSource = EntryManager.GetBlogEntries(Request.QueryString["tag"], TotalToShow);
                ListView1.ItemDataBound += ListView1_ItemDataBound;
                ListView1.DataBind();
            }
            else
            {
                ListView1.DataSource = EntryManager.GetBlogEntries(Sitecore.Context.Item.ID, TotalToShow);
                ListView1.ItemDataBound += ListView1_ItemDataBound;
                ListView1.DataBind();
            }
        }

        #endregion

        #region Eventhandlers

        private void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            Entry objEntry = (Entry) ((ListViewDataItem) e.Item).DataItem;

            PlaceHolder PostedDate = (PlaceHolder) e.Item.FindControl("PostedDate");
            PostedDate.Controls.Add(new LiteralControl(objEntry.Created.ToString("dddd, MMMM d, yyyy")));

            Web.UI.WebControls.Text txtTitle = (Web.UI.WebControls.Text) e.Item.FindControl("txtTitle");
            txtTitle.DataSource = objEntry.ID.ToString();

            Web.UI.WebControls.Text txtIntroduction = (Web.UI.WebControls.Text) e.Item.FindControl("txtIntroduction");
            txtIntroduction.DataSource = objEntry.ID.ToString();

            HyperLink postLink = (HyperLink) e.Item.FindControl("BlogPostLink");
            postLink.NavigateUrl = LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(objEntry.ID));

            if (!objEntry.DisableComments)
            {
                PlaceHolder commentsPlaceholder = (PlaceHolder) e.Item.FindControl("CommentsPlaceholder");
                commentsPlaceholder.Controls.Add(
                    new LiteralControl("Comments " + "(" + CommentManager.GetCommentsCount(objEntry.ID) + ")"));
            }
        }

        #endregion
    }
}