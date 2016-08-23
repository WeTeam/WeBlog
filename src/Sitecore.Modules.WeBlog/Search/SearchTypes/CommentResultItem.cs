using Sitecore.ContentSearch.SearchTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch;

namespace Sitecore.Modules.WeBlog.Search.SearchTypes
{
    public class CommentResultItem : SearchResultItem
    {
        [IndexField("full_created_date")]
        public DateTime FullCreatedDate { get; set; }
    }
}