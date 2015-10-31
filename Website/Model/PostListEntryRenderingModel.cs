using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Model
{
    public class PostListEntryRenderingModel
    {
        public string PostTemplate { get; set; }
        public EntryItem EntryItem { get; set; }
        public string Summary { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
    }
}