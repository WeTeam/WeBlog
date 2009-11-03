using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Data;

namespace Sitecore.Modules.Eviblog.Items
{
    public class Settings : CustomItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Blog"/> class.
        /// </summary>
        /// <param name="item">The blog.</param>
        public Settings(Item item) : base(item)
        {
        }

        /// <summary>
        /// Gets or sets the blog ID.
        /// </summary>
        /// <value>The blog ID.</value>
        public ID BlogID
        {
            get
            {
                return new ID(InnerItem["BlogID"]);
            }
            set
            {
                InnerItem["BlogID"] = value.ToString();
            }
        }
    }
}