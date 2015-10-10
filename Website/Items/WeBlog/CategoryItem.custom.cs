using Sitecore.Links;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public partial class CategoryItem
    {
        /// <summary>
        /// Gets the title of the entry or the name if the title is empty
        /// </summary>
        public string DisplayTitle
        {
            get
            {
                if (!string.IsNullOrEmpty(Title.Text))
                    return Title.Text;

                return Name;
            }
        }

        /// <summary>
        /// Gets the URL for a category
        /// </summary>
        /// <returns>The URL of the category</returns>
        public object GetUrl()
        {
            return LinkManager.GetItemUrl(Context.Database.GetItem(ID));
        }
    }
}