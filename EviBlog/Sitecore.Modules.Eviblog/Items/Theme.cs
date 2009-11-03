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
