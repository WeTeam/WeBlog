using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;

namespace Sitecore.Modules.Blog.Utilities
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
            if (item == null || template == null)
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
            if (itemTemplate == null || baseTemplate == null)
                return false;

            if (itemTemplate.ID == baseTemplate.ID)
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
    }
}
