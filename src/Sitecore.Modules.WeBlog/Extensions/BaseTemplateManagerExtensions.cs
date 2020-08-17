using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Extensions
{
    public static class BaseTemplateManagerExtensions
    {
        /// <summary>
        /// Determine if an item is based on a given template or if the item's template is based on the given template
        /// </summary>
        /// <param name="templateManager">The template manager to use to locate templates.</param>
        /// <param name="item">The item to test the template of.</param>
        /// <param name="templateId">The ID of the template which the item's template should be or inherit from.</param>
        /// <returns>True if the item's template is based on the given templates, otherwise false.</returns>
        public static bool TemplateIsOrBasedOn(this BaseTemplateManager templateManager, Item item, ID templateId)
        {
            return templateManager.TemplateIsOrBasedOn(item, new[] { templateId });
        }

        /// <summary>
        /// Determine if an item is based on a given template or if the item's template is based on the given template
        /// </summary>
        /// <param name="templateManager">The template manager to use to locate templates.</param>
        /// <param name="item">The item to test the template of.</param>
        /// <param name="templateIds">The IDs of the templates which the item's template should be or inherit from.</param>
        /// <returns>True if the item's template is based on the given templates, otherwise false.</returns>
        public static bool TemplateIsOrBasedOn(this BaseTemplateManager templateManager, Item item, IEnumerable<ID> templateIds)
        {
            if (item == null || templateIds == null || !templateIds.Any())
                return false;

            var template = templateManager.GetTemplate(item.TemplateID, item.Database);
            if (template == null)
                return false;

            var match = from templateId in templateIds
                        where template.DescendsFromOrEquals(templateId)
                        select templateId;

            return match.Any();
        }
    }
}
