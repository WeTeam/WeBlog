using Sitecore.Modules.WeBlog.Components.Archive;

namespace Sitecore.Modules.WeBlog.Model
{
    public class ArchiveRenderingModel
    {
        public bool ExpandMonthsOnLoad { get; set; }
        public bool ExpandPostsOnLoad { get; set; }
        public IArchiveCore ArchiveCore { get; set; }
    }
}