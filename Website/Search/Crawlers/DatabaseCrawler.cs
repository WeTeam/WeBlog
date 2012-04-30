using System;
using Sitecore.Modules.WeBlog.Managers;
using Lucene.Net.Documents;
using Sitecore.Data;
using System.Collections.Generic;
using System.Text;

namespace Sitecore.Modules.WeBlog.Search.Crawlers
{
    public class DatabaseCrawler : Sitecore.Search.Crawlers.DatabaseCrawler
    {
        private List<string> m_multilistFields = null;
        private List<string> m_dataFieldNames = null;

        public DatabaseCrawler()
        {
            m_multilistFields = new List<string>();
            m_dataFieldNames = new List<string>();
        }
        
        protected override void AddAllFields(Lucene.Net.Documents.Document document, Data.Items.Item item, bool versionSpecific)
        {
            base.AddAllFields(document, item, versionSpecific);

            if (item != null && document != null)
            {
                // Sitecore 6.2 does not include template
                document.Add(new Field(Constants.Index.Fields.Template, TransformValue(item.TemplateID), Field.Store.NO, Field.Index.TOKENIZED));

                // Add publishable field to allow for publishing restrictions
                document.Add(new Field(Constants.Index.Fields.Publish, Sitecore.DateUtil.ToIsoDate(item.Publishing.PublishDate), Field.Store.NO, Field.Index.TOKENIZED));

                // Add multilist fields
                foreach (var fieldName in m_multilistFields)
                {
                    if(item.Fields[fieldName] != null)
                        document.Add(new Field(fieldName, TransformMultilistValue(item.Fields[fieldName]), Field.Store.YES, Field.Index.TOKENIZED));
                }

                // Add additional fields
                foreach (var fieldName in m_dataFieldNames)
                {
                    if (item.Fields[fieldName] != null)
                    {
                        document.Add(new Field(fieldName, TransformCSV(item.Fields[fieldName].Value), Field.Store.YES, Field.Index.TOKENIZED));
                    }
                }
            }
        }

        /// <summary>
        /// Transforms a value for use in the index
        /// </summary>
        /// <param name="id">The ID to transform</param>
        /// <returns>A value suitable for usage with the index</returns>
        protected virtual string TransformValue(ID id)
        {
            return id.ToShortID().ToString().ToLower();
        }

        /// <summary>
        /// Tranforms a multilist field for use in the index
        /// </summary>
        /// <param name="field">The field tgo transform</param>
        /// <returns>A value suitable for usage with the index</returns>
        protected virtual string TransformMultilistValue(Sitecore.Data.Fields.MultilistField field)
        {
            var buffer = new StringBuilder();

            if (field != null)
            {
                foreach (string value in field)
                {
                    var id = ID.Null;
                    if (ID.TryParse(value, out id))
                        buffer.Append(TransformValue(id));
                    else
                        buffer.Append(value);

                    buffer.Append(" ");
                }
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Registers a multilist field for indexing
        /// </summary>
        /// <param name="field">The name of the field</param>
        protected virtual void AddMultilistField(string field)
        {
            m_multilistFields.Add(field);
        }

        /// <summary>
        /// Registers a data field for indexing
        /// </summary>
        /// <param name="field">The name of the field</param>
        protected virtual void AddDataField(string field)
        {
            m_dataFieldNames.Add(field);
        }

        /// <summary>
        /// Transforms an input to remove the whitespace and allow tokenising on other characters
        /// </summary>
        /// <param name="value">The string to transform</param>
        /// <returns>The transformed string</returns>
        public static string TransformCSV(string value)
        {
            var collapsed = value.Replace(" ", string.Empty);
            return collapsed.Replace(',', ' ');
        }
    }
}