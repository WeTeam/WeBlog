using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class RecentCommentsRenderingModel
    {
        public CommentContent Comment { get; set; }
        public string EntryTitle { get; set; }
        public string EntryUrl { get; set; }
    }
}