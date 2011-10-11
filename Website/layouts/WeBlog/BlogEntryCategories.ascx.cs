using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Layouts;
using Sitecore.Modules.WeBlog.Items.Blog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace Sitecore.Modules.WeBlog.layouts.WeBlog
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
            ListViewCategories.DataSource = CategoryManager.GetCategoriesByEntryID(current.ID);
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