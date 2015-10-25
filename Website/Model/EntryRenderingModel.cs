using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Model
{
    public class EntryRenderingModel
    {
        public EntryItem CurrentEntry { get; set; }
        public string Title { get; set; }
        public bool ShowEntryIntroduction { get; set; }
        public bool ShowEntryMetadata { get; set; }
        public bool ShowEntryImage { get; set; }
        public bool ShowEntryTitle { get; set; }

        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
    }
}