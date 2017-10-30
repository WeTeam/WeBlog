using System.ComponentModel;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;

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
            EntryNavigationCore = entryNavigationCore ?? new EntryNavigationCore(GetDefaultPostListCore());
            NextEntry = EntryNavigationCore.GetNextEntry(CurrentEntry);
            PreviousEntry = EntryNavigationCore.GetPreviousEntry(CurrentEntry);
        }

        protected PostListCore GetDefaultPostListCore()
        {
            var defaultPostListCore = new PostListCore(CurrentBlog);
            defaultPostListCore.Initialize(System.Web.HttpContext.Current.Request.QueryString);
            return defaultPostListCore;
        }
    }
}