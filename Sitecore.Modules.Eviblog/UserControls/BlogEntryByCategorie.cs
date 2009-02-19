using System;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public partial class BlogEntryByCategorie : System.Web.UI.UserControl
    {
        #region Fields
        protected ListView BlogEntriesByCategorieListView;
        #endregion

        #region Page Methods
        protected void Page_Load(object sender, EventArgs e)
        {
            Sitecore.Data.ID BlogID = BlogManager.GetCurrentBlogID();
            string CategorieName = Sitecore.Context.Item.Name;

            BlogEntriesByCategorieListView.DataSource = EntryManager.GetBlogEntryByCategorie(BlogID, CategorieName);
            BlogEntriesByCategorieListView.ItemDataBound += new EventHandler<ListViewItemEventArgs>(ListView1_ItemDataBound);
            BlogEntriesByCategorieListView.DataBind();
        }
        #endregion

        #region Eventhandlers
        void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            Entry objEntry = (Entry)((ListViewDataItem)e.Item).DataItem;

            Sitecore.Web.UI.WebControls.Text txt = (Sitecore.Web.UI.WebControls.Text)e.Item.FindControl("Text1");
            txt.DataSource = objEntry.ID.ToString();

            HyperLink postLink = (HyperLink)e.Item.FindControl("BlogPostLink");
            postLink.NavigateUrl = LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(objEntry.ID));
        }
        #endregion
    }
}