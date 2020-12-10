using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Extensions
{
    /// <summary>
    /// Provides utilities for working with Sitecore items
    /// </summary>
    public static class ItemExtensions
    {
        /// <summary>
        /// Find items below another based on the specified template or a derived template
        /// </summary>
        /// <param name="rootItem">The item the item must be below</param>
        /// <param name="template">The template to find item based on, or based on derivaties of the template</param>
        /// <returns>All items which match</returns>
        public static Item[] FindItemsByTemplateOrDerivedTemplate(this Item rootItem, TemplateItem template)
        {
            if (rootItem == null || template == null)
                return new Item[0];

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
                    if (source.Axes.IsDescendantOf(rootItem))
                        foundItems.Add(source);
                }
            }

            foreach (var derivedTemplate in derivedTemplates)
            {
                foundItems.AddRange(FindItemsByTemplateOrDerivedTemplate(rootItem, derivedTemplate));
            }

            return foundItems.ToArray();
        }

        /// <summary>
        /// Finds the current type of item for the given item
        /// </summary>
        /// <param name="item">The item to search from</param>
        /// <param name="templateId">The template the target item must be based on or derived from</param>
        /// <param name="templateManager">The <see cref="BaseTemplateManager"/> used to access templates.</param>
        /// <returns>The target item if found, otherwise null</returns>
        public static Item FindAncestorByTemplate(this Item item, ID templateId, BaseTemplateManager templateManager)
        {
            if (item == null)
                return null;

            if (templateManager == null)
                return null;

            var currentItem = item;

            while (currentItem != null && !templateManager.TemplateIsOrBasedOn(currentItem, templateId))
            {
                currentItem = currentItem.Parent;
            }

            return currentItem;
        }

        /// <summary>
        /// Finds the current type of item for the given item
        /// </summary>
        /// <param name="item">The item to search from</param>
        /// <param name="templateIds">The template the target item must be based on or derived from</param>
        /// <param name="templateManager">The <see cref="BaseTemplateManager"/> used to access templates.</param>
        /// <returns>The target item if found, otherwise null</returns>
        public static Item FindAncestorByAnyTemplate(this Item item, IEnumerable<ID> templateIds, BaseTemplateManager templateManager)
        {
            if (item == null)
                return null;

            if (templateManager == null)
                return null;

            var currentItem = item;

            while (currentItem != null && !templateManager.TemplateIsOrBasedOn(currentItem, templateIds))
            {
                currentItem = currentItem.Parent;
            }

            return currentItem;
        }

        /// <summary>
        /// Gets the content of the title field if not empty, otherwise the item name
        /// </summary>
        /// <param name="item">The item to get the title for</param>
        /// <returns>The title if not empty, otherwise the item's name</returns>
        public static string GetItemTitle(this Item item)
        {
            if (item != null)
            {
                var title = item["title"];
                return string.IsNullOrEmpty(title) ? item.Name : title;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the URL for an item
        /// </summary>
        /// <param name="item">The item to get the URL for</param>
        /// <returns>The URL for the item if valid, otherwise an empty string</returns>
        public static string GetUrl(this Item item)
        {
            return LinkManager.GetItemUrl(item);
        }

        /// <summary>
        /// Determines if the field given by name needs to have it's output wrapped in an additional tag
        /// </summary>
        /// <param name="item">The item to check field on</param>
        /// <param name="fieldName">The name of the field to check</param>
        /// <returns>True if wrapping is required, otherwsie false  </returns>
        public static bool DoesFieldRequireWrapping(this Item item, string fieldName)
        {
            return !item[fieldName].StartsWith("<p>");
        }
    }
}
