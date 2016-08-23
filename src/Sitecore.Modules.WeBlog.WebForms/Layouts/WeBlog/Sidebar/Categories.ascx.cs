using System;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar
{
    public partial class BlogCategories : BaseSublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadCategories();
        }

        /// <summary>
        /// Load the categories into this control
        /// </summary>
        protected virtual void LoadCategories()
        {
            if (ManagerFactory.CategoryManagerInstance.GetCategories().Length == 0)
            {
                if (PanelCategories != null)
                    PanelCategories.Visible = false;
            }
            else
            {
                if (ListViewCategories != null)
                {
                    ListViewCategories.DataSource = ManagerFactory.CategoryManagerInstance.GetCategories();
                    ListViewCategories.DataBind();
                }
            }
        }
    }
}