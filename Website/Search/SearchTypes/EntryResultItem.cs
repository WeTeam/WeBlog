using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Search.SearchTypes
{
    public class EntryResultItem : SearchResultItem
    {

        [IndexField("Category")]
        public ID[] Category { get; set; }

        [IndexField("Tags")]
        public string Tags { get; set; }
    }
}