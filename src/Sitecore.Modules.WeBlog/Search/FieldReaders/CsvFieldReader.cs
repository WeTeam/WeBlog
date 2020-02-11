using Sitecore.ContentSearch;
using Sitecore.ContentSearch.FieldReaders;
using System.Linq;

namespace Sitecore.Modules.WeBlog.Search.FieldReaders
{
    /// <summary>
    /// A <see cref="FieldReader"/> which parses values from a CSV field value.
    /// </summary>
    public class CsvFieldReader : FieldReader
    {
        public override object GetFieldValue(IIndexableDataField field)
        {
            var tags = field.Value.ToString().Split(',');
            
            for(var i = 0; i < tags.Length; i++)
            {
                tags[i] = tags[i].Trim();
            }

            return tags.ToList();
        }
    }
}