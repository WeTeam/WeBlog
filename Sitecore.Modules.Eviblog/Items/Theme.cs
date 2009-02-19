using Sitecore.Data.Items;

namespace Sitecore.Modules.Eviblog.Items
{
    public class Theme : CustomItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Theme"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public Theme(Item item): base(item)
        {
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The title.</value>
        public string Name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        public string File
        {
            get
            {
                return InnerItem["File location"];
            }
            set
            {
                InnerItem["File location"] = value;
            }
            
        }
    }
}
