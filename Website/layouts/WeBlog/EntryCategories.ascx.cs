using System;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogEntryCategories : BaseEntrySublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadEntry();
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        protected virtual void LoadEntry()
        {
            // Fill categories
            ListViewCategories.DataSource = ManagerFactory.CategoryManagerInstance.GetCategoriesByEntryID(CurrentEntry.ID);
            ListViewCategories.DataBind();

            //TODO Create edit possibilities for assigning categories on frontend
        }
    }
}