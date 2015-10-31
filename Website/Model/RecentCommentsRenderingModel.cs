using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Model
{
    public class RecentCommentsRenderingModel
    {
        public CommentItem CommentItem { get; set; }
        public string EntryTitle { get; set; }
        public string EntryUrl { get; set; }
    }
}