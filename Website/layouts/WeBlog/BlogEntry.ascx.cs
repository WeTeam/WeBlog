using System;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Items.Blog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Globalization;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogEntry : BaseEntrySublayout
    {
        #region Page methods

        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.Presentation.SetProperties(this);

            LoadEntry();
            Page.Title = CurrentEntry.Title.Text + " | " + CurrentBlog.Title.Text;

            var maxEntryImage = CurrentBlog.MaximumEntryImageSizeDimension;
            if(maxEntryImage != Size.Empty)
            {
                EntryImage.MaxWidth = maxEntryImage.Width;
                EntryImage.MaxHeight = maxEntryImage.Height;
            }
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

            LoadTags(current);
        }

        #endregion

        /// <summary>
        /// Load the tags for the current entry
        /// </summary>
        /// <param name="entry">The entry to laod the tags from</param>
        protected virtual void LoadTags(EntryItem entry)
        {
            if (!Sitecore.Context.PageMode.IsPageEditorEditing)
            {
                var tags = TagManager.GetTagsByEntry(entry);
                var list = LoginViewTags.FindControl("TagList") as Repeater;

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
            return LinkManager.GetItemUrl(Sitecore.Context.Item.Parent) + "?tag=" + tag; ;
        }

        #region Eventhandlers

        

        #endregion

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