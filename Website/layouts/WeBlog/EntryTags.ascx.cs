using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Layouts;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Links;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.layouts.WeBlog
{
    public partial class BlogEntryTags : BaseEntrySublayout
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

            // Load tags
            if (!Sitecore.Context.PageMode.IsPageEditorEditing)
            {
                var tags = ManagerFactory.TagManagerInstance.GetTagsByEntry(current);
                var list = LoginViewTags.FindControl("TagList") as ListView;

                if (list != null)
                {
                    list.DataSource = from tag in tags select tag.Key;
                    list.DataBind();
                }
            }
        }
        
        /// <summary>
        /// Get the URL for a tag
        /// </summary>
        /// <param name="tag">The tag to get the URL for</param>
        /// <returns>The URL to the tag</returns>
        protected virtual string GetTagUrl(string tag)
        {
            return LinkManager.GetItemUrl(CurrentBlog.InnerItem) + "?tag=" + tag; ;
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