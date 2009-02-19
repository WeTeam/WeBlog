using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogCategories : UserControl
    {
        #region Fields

        public Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        protected ListView ListViewCategories;
        protected Panel PanelCategories;
        protected Web.UI.WebControls.Text Text;
        protected Web.UI.WebControls.Text titleCategories;

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            titleCategories.Item = BlogManager.GetCurrentBlogItem();

            if (CategoryManager.GetCategories().Count == 0)
            {
                PanelCategories.Visible = false;
            }
            else
            {
                ListViewCategories.DataSource = CategoryManager.GetCategories();
                ListViewCategories.ItemDataBound += ListViewCategories_ItemDataBound;
                ListViewCategories.DataBind();
            }
        }

        #endregion

        #region Eventhandlers

        private void ListViewCategories_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            Category objCategory = (Category) ((ListViewDataItem) e.Item).DataItem;

            Web.UI.WebControls.Text txt = (Web.UI.WebControls.Text) e.Item.FindControl("txtCategorie");
            txt.DataSource = objCategory.ID.ToString();

            HyperLink postLink = (HyperLink) e.Item.FindControl("hyperlinkCategory");
            postLink.NavigateUrl = LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(objCategory.ID));
        }

        #endregion
    }
}