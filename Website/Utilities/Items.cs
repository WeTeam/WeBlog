using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Utilities
{
    /// <summary>
    /// Provides utilities for working with Sitecore items
    /// </summary>
    public static class Items
    {
        private static readonly string SAFE_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789- ";

        /// <summary>
        /// Generates a safe item name from the given input which can be used to create an item in the content tree
        /// </summary>
        /// <param name="input">The desired item name</param>
        /// <returns>A safe item name</returns>
        public static string MakeSafeItemName(string input)
        {
            var output = new StringBuilder();
            foreach (var c in input)
            {
                if (SAFE_CHARS.Contains(c))
                    output.Append(c);
                else
                    output.Append('-');
            }

            return output.ToString();
        }

        /// <summary>
        /// Determine if an item is based on a given template or if the item's template is based on the given template
        /// </summary>
        /// <param name="item">The item to test the template of</param>
        /// <param name="template">The template which the item's template should be or inherit from</param>
        /// <returns>True if the item's template is based on the given template, otherwsie false</returns>
        public static bool TemplateIsOrBasedOn(Item item, TemplateItem template)
        {
            return TemplateIsOrBasedOn(item, template.ID);
        }

        /// <summary>
        /// Determine if an item is based on a given template or if the item's template is based on the given template
        /// </summary>
        /// <param name="item">The item to test the template of</param>
        /// <param name="template">The ID of the template which the item's template should be or inherit from</param>
        /// <returns>True if the item's template is based on the given template, otherwsie false</returns>
        public static bool TemplateIsOrBasedOn(Item item, ID template)
        {
            if (item == null || template == ID.Null)
                return false;

            return TemplateIsOrBasedOn(item.Template, template);
        }

        /// <summary>
        /// Determine if an item is based on a given template or if the item's template is based on the given template
        /// </summary>
        /// <param name="item">The item to test the template of</param>
        /// <param name="template">The template which the item's template should be or inherit from</param>
        /// <returns>True if the item's template is based on the given template, otherwsie false</returns>
        public static bool TemplateIsOrBasedOn(TemplateItem itemTemplate, TemplateItem baseTemplate)
        {
            return TemplateIsOrBasedOn(itemTemplate, baseTemplate.ID);
        }

        /// <summary>
        /// Determine if an item is based on a given template or if the item's template is based on the given template
        /// </summary>
        /// <param name="item">The item to test the template of</param>
        /// <param name="template">The ID of the template which the item's template should be or inherit from</param>
        /// <returns>True if the item's template is based on the given template, otherwsie false</returns>
        public static bool TemplateIsOrBasedOn(TemplateItem itemTemplate, ID baseTemplate)
        {
            if (itemTemplate == null || baseTemplate == ID.Null)
                return false;

            if (itemTemplate.ID == baseTemplate)
                return true;

            else
            {
                bool result = false;
                foreach(var template in itemTemplate.BaseTemplates)
                {
                    result = TemplateIsOrBasedOn(template, baseTemplate);
                    if (result)
                        return result;
                }
            }

            return false;
        }

        /// <summary>
        /// Find items below another based on the specified template or a derived template
        /// </summary>
        /// <param name="rootItem">The item the item must be below</param>
        /// <param name="template">The template to find item based on, or based on derivaties of the template</param>
        /// <returns>All items which match</returns>
        public static Item[] FindItemsByTemplateOrDerivedTemplate(Item rootItem, TemplateItem template)
        {
            if (rootItem == null || template == null)
                return new Item[0];

            var database = rootItem.Database;
            var foundItems = new List<Item>();
            var derivedTemplates = new List<TemplateItem>();

            var references = Sitecore.Globals.LinkDatabase.GetReferrers(template);
            foreach (var reference in references)
            {
                var source = reference.GetSourceItem();
                if (source.Template.Key == "template")
                    derivedTemplates.Add((TemplateItem)source);
                else
                {
                    if(source.Axes.IsDescendantOf(rootItem))
                        foundItems.Add(source);
                }
            }

            foreach(var derivedTemplate in derivedTemplates)
            {
                foundItems.AddRange(FindItemsByTemplateOrDerivedTemplate(rootItem, derivedTemplate));
            }

            return foundItems.ToArray();
        }

        /// <summary>
        /// Finds the current type of item for the given item
        /// </summary>
        /// <param name="item">The item to search from</param>
        /// <param name="template">The template the target item must be based on or derived from</param>
        /// <returns>The target item if found, otherwise null</returns>
        public static Item GetCurrentItem(Item item, string template)
        {
            if (item == null)
                return null;

            var templateValue = item.Database.GetTemplate(template);
            var currentItem = item;

            while (currentItem != null && !Utilities.Items.TemplateIsOrBasedOn(currentItem, templateValue))
            {
                currentItem = currentItem.Parent;
            }

            if (currentItem != null)
                return currentItem;
            else
                return null;
        }
    }
}
