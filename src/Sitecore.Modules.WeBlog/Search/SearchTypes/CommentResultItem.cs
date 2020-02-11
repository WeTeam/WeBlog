using Sitecore.ContentSearch.SearchTypes;
using System;
using Sitecore.ContentSearch;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Search.SearchTypes
{
    public class CommentResultItem : SearchResultItem
    {
        [IndexField("full_created_date")]
        public DateTime FullCreatedDate { get; set; }

        [IndexField("closest_entry_uri")]
        public ItemUri EntryUri { get; set; }
    }
}