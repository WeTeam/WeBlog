using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class InterestingEntries : BlogRenderingModelBase
    {
        protected IInterstingEntriesAlgorithm InterestingEntriesCore { get; set; }

        public string Mode { get; set; }

        public int MaximumCount { get; set; }

        public EntryItem[] Entries { get; set; }

        public InterestingEntries() : this(null) { }

        public InterestingEntries(IInterstingEntriesAlgorithm interestingEntriesCore)
        {
            InterestingEntriesCore = interestingEntriesCore;
            var algororithm = Components.InterestingEntriesCore.GetAlgororithmFromString(Mode);
            if (algororithm != InterestingEntriesAlgorithm.Custom || InterestingEntriesCore == null)
            {
                InterestingEntriesCore = new InterestingEntriesCore(ManagerFactory.EntryManagerInstance, algororithm);
            }
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            Entries = InterestingEntriesCore.GetEntries(CurrentBlog, MaximumCount);
        }
    }
}