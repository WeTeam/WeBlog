using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Converters;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Search.SearchTypes
{
    public class EntryResultItem : SearchResultItem
    {
        [IndexField("title")]
        public virtual string Title { get; set; }

        [IndexField("category")]
        public virtual ID[] Category { get; set; }

        [IndexField("tags")]
        public virtual string[] Tags { get; set; }

        [IndexField("entry_date")]
        public virtual DateTime EntryDate { get; set; }
    }
}