using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class RecentCommentsRenderingModel
    {
        public CommentItem CommentItem { get; set; }
        public string EntryTitle { get; set; }
        public string EntryUrl { get; set; }
    }
}