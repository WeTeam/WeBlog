using System;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Search.SearchTypes
{
    public class EntryResultItem : SearchResultItem
    {
#if SC70
      [IndexField("category")]
      public string Category { get; set; }
#else
        [IndexField("category")]
        public ID[] Category { get; set; }
#endif

        [IndexField("tags")]
        public string Tags { get; set; }

        [IndexField("entry_date")]
        public DateTime EntryDate { get; set; }
    }
}