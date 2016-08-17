using System;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Model
{
    public class TemplatesMapping
    {
        public ID BlogRootTemplate { get; set; }
        public ID BlogEntryTemplate { get; set; }
        public ID BlogCategoryTemplate { get; set; }
        public ID BlogCommentTemplate { get; set; }

        public TemplatesMapping(Item templatesMappingItem)
        {
            BlogRootTemplate = GetId(templatesMappingItem, Templates.TemplateMapping.Fields.BlogRootTemplate);
            BlogEntryTemplate = GetId(templatesMappingItem, Templates.TemplateMapping.Fields.BlogEntryTemplate);
            BlogCategoryTemplate = GetId(templatesMappingItem, Templates.TemplateMapping.Fields.BlogCategoryTemplate);
            BlogCommentTemplate = GetId(templatesMappingItem, Templates.TemplateMapping.Fields.BlogCommentTemplate);
        }

        protected ID GetId(Item templatesMappingItem, ID fieldName)
        {
            var fieldValue = GetFieldValue(templatesMappingItem, fieldName);
            return !String.IsNullOrEmpty(fieldValue) ? new ID(fieldValue) : ID.Null;
        }

        protected string GetFieldValue(Item templatesMappingItem, ID fieldName)
        {
            return templatesMappingItem.Fields[fieldName]?.Value;
        }
    }
}