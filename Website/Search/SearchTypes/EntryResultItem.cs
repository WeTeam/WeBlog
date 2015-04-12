using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Modules.WeBlog.Search.SearchTypes
{
    public class EntryResultItem : SearchResultItem
    {

        [IndexField("Category")]
        public string Category { get; set; }

        [IndexField("Tags")]
        public string Tags { get; set; }
    }
}