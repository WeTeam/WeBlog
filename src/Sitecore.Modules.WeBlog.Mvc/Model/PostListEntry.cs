using System.Drawing;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;
using Sitecore.Pipelines;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class PostListEntry
    {
        public EntryItem EntryItem { get; set; }
        public string Summary { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public bool ShowCommentsCount { get; set; }
        public int CommentCount { get; protected set; }

        public PostListEntry(EntryItem entry, ICommentManager commentManager)
        {
            EntryItem = entry;
            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(entry);
            Size maxEntryImage = currentBlog.MaximumThumbnailImageSizeDimension;
            if (maxEntryImage != Size.Empty)
            {
                MaxWidth = maxEntryImage.Width;
                MaxHeight = maxEntryImage.Height;
            }
            Summary = GetSummary(EntryItem);

            ShowCommentsCount = currentBlog.EnableComments.Checked && !EntryItem.DisableComments.Checked;

            CommentCount = commentManager.GetCommentsCount(EntryItem);
        }

        protected string GetSummary(EntryItem entry)
        {
            var args = new GetSummaryArgs();
            args.Entry = entry;

            CorePipeline.Run("weblogGetSummary", args, true);

            return args.Summary;
        }
    }
}