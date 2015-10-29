using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Model
{
    public class PostListRenderingModel
    {
        public IEnumerable<EntryItem> Entries { get; set; }
        public bool ShowViewMoreLink { get; set; }
        public string ViewMoreHref { get; set; }
        public string PostTemplate { get; set; }
    }
}