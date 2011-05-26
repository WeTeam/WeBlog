using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Modules.Blog.Items.Blog;
using Sitecore.Links;
using Sitecore.Modules.Blog.Items;
using Sitecore.Modules.Blog.Managers;

namespace Sitecore.Modules.Blog.Layouts
{
    public partial class BlogCategories : BaseSublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                titleCategories.Item = CurrentBlog.InnerItem;
                LoadCategories();
            }
        }

        /// <summary>
        /// Load the categories into this control
        /// </summary>
        protected virtual void LoadCategories()
        {
            if (CategoryManager.GetCategories().Length == 0)
            {
                PanelCategories.Visible = false;
            }
            else
            {
                ListViewCategories.DataSource = CategoryManager.GetCategories();
                ListViewCategories.DataBind();
            }
        }

        /// <summary>
        /// Get the URL for a category
        /// </summary>
        /// <param name="category">The category to get the URL for</param>
        /// <returns>The URL if the category is valid, otherwise an emtpy string</returns>
        protected virtual string GetCategoryUrl(CategoryItem category)
        {
            if (category != null)
                return LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(category.ID));
            else
                return string.Empty;
        }
    }
}