using System;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Data.Items;
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
            // Create entry of current item
            EntryItem current = new EntryItem(Sitecore.Context.Item);

            // Fill categories
            ListViewCategories.DataSource = ManagerFactory.CategoryManagerInstance.GetCategoriesByEntryID(current.ID);
            ListViewCategories.DataBind();

            //TODO Create edit possibilities for assigning categories on frontend
        }

        /// <summary>
        /// Get the URL for an item
        /// </summary>
        /// <param name="item">The item to get the URL for</param>
        /// <returns>The URL for the item if valid, otherwise an empty string</returns>
        protected virtual string GetItemUrl(Item item)
        {
            if (item != null)
                return LinkManager.GetItemUrl(item);
            else
                return string.Empty;
        }
    }
}