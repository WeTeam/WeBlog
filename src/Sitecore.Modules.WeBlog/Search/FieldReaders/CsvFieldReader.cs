using Sitecore.ContentSearch;
using Sitecore.ContentSearch.FieldReaders;

namespace Sitecore.Modules.WeBlog.Search.FieldReaders
{
    /// <summary>
    /// A <see cref="FieldReader"/> which parses values from a CSV field value.
    /// </summary>
    public class CsvFieldReader : FieldReader
    {
        public override object GetFieldValue(IIndexableDataField field)
        {
            return field.Value.ToString().Split(',');
        }
    }
}