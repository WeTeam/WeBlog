using System.ComponentModel;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class EntryNavigation : BlogRenderingModelBase
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

        protected PostListCore GetDefaultPostListCore()
        {
            var defaultPostListCore = new PostListCore(CurrentBlog, null, null, null, null);
            var queryString = RenderingContext.Current.PageContext.RequestContext.HttpContext.Request.QueryString;
            defaultPostListCore.Initialize(queryString);
            return defaultPostListCore;
        }
    }
}