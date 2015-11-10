using System.ComponentModel;
using Sitecore.Modules.WeBlog.Components.Archive;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Model
{
    public class Archive : BlogRenderingModelBase
    {
        public IArchiveCore ArchiveCore { get; set; }

        public Archive() : this(null) { }

        public Archive(IArchiveCore archiveCore)
        {
            ArchiveCore = archiveCore ?? new ArchiveCore(ManagerFactory.BlogManagerInstance);
        }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ExpandMonthsOnLoad { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ExpandPostsOnLoad { get; set; }
    }
}