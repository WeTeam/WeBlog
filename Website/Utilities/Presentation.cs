using System;
using System.Collections.Generic;
using System.Web.UI;
using Sitecore.Web;
using Sitecore.Web.UI.WebControls;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Utilities
{
	public static class Presentation
	{
        /// <summary>
        /// Gets the content of the title field if not empty, otherwise the item name
        /// </summary>
        /// <param name="item">The item to get the title for</param>
        /// <returns>The title if not empty, otherwise the item's name</returns>
        public static string GetItemTitle(Item item)
        {
            if (item != null)
            {
                var title = item["title"];
                return string.IsNullOrEmpty(title) ? item.Name : title;
            }

            return string.Empty;
        }
	}
}
