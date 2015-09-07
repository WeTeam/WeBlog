using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Search.SearchTypes
{
    public class EntryResultItem : SearchResultItem
    {
#if SC70
      [IndexField("Category")]
      public string Category { get; set; }
#else
        [IndexField("Category")]
        public ID[] Category { get; set; }
#endif

        [IndexField("Tags")]
        public string Tags { get; set; }
    }
}