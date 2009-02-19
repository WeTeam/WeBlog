using Sitecore.Data.Items;

namespace Sitecore.Modules.Eviblog.Items
{
    public class Category : CustomItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public Category(Item item): base(item)
        {
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return InnerItem["Title"];
            }
            set
            {
                InnerItem["Title"] = value;
            }
        }
    }
}
