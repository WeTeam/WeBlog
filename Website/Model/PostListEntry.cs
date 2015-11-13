using System.Drawing;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;
using Sitecore.Pipelines;

namespace Sitecore.Modules.WeBlog.Model
{
    public class PostListEntry
    {
        public EntryItem EntryItem { get; set; }
        public string Summary { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public PostListEntry(EntryItem entry)
        {
            EntryItem = entry;
            Size maxEntryImage = ManagerFactory.BlogManagerInstance.GetCurrentBlog(entry).MaximumThumbnailImageSizeDimension;
            if (maxEntryImage != Size.Empty)
            {
                MaxWidth = maxEntryImage.Width;
                MaxHeight = maxEntryImage.Height;
            }
            Summary = GetSummary(EntryItem);
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