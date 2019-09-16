using System.ComponentModel;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    public partial class EntryNavigation : BaseEntrySublayout
    {
        protected IEntryNavigationCore EntryNavigationCore { get; set; }
        public EntryItem NextEntry { get; set; }
        public EntryItem PreviousEntry { get; set; }
        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowTitle { get; set; }

        public EntryNavigation() : this(null) { }

        public EntryNavigation(IEntryNavigationCore entryNavigationCore)
        {
            EntryNavigationCore = entryNavigationCore ?? new EntryNavigationCore(ManagerFactory.BlogManagerInstance, ManagerFactory.EntryManagerInstance);
            NextEntry = EntryNavigationCore.GetNextEntry(CurrentEntry);
            PreviousEntry = EntryNavigationCore.GetPreviousEntry(CurrentEntry);
        }
    }
}