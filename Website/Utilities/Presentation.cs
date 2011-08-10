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
		/// Set properties on a control from Sitecore parameters
		/// </summary>
		/// <param name="control">The control to set the properties on</param>
		public static void SetProperties(UserControl control)
		{
			// Get the parameters from the sublayout
			var container = (Sublayout)control.Parent;
			var parameters = WebUtil.ParseQueryString(container.Parameters);

			// Enumerate each parameter
			foreach (KeyValuePair<string, string> entry in parameters)
			{
				// Try and find a property of the same name as the entry on the control
				var propertyInfo = control.GetType().GetProperty(entry.Key);

				// If found, set the property
				if (propertyInfo != null)
				{
					object value = null;

					if (propertyInfo.PropertyType == typeof(bool))
					{
						var boolValue = false;
						if (bool.TryParse(entry.Value, out boolValue))
							value = boolValue;
						else if (entry.Value == "1")
							value = true;
						else
							value = false;
					}
					else if (propertyInfo.PropertyType == typeof(DateTime))
					{
						var dateValue = DateTime.MinValue;
						if (DateTime.TryParse(entry.Value, out dateValue))
							value = dateValue;
					}
					else
						value = entry.Value;

					if (value != null)
					{
						propertyInfo.SetValue(control, value, null);
					}
				}
			}
		}

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
