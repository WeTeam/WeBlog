using System;
using System.Linq;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar
{
    public partial class BlogInterestingEntries : BaseSublayout
    {
        public IInterstingEntriesAlgorithm InterestingEntriesCore { get; set; }

        /// <summary>
        /// Gets or sets the algorithm to use in a textual fashion
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items to show
        /// </summary>
        public int MaximumCount { get; set; }

        public BlogInterestingEntries(IInterstingEntriesAlgorithm interestingEntriesCore = null)
        {
            InterestingEntriesCore = interestingEntriesCore;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var algororithm = WeBlog.Components.InterestingEntriesCore.GetAlgororithmFromString(Mode);
            if (algororithm != InterestingEntriesAlgorithm.Custom || InterestingEntriesCore == null)
            {
                InterestingEntriesCore = new InterestingEntriesCore(ManagerFactory.EntryManagerInstance, algororithm);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var dataSource = InterestingEntriesCore.GetEntries(CurrentBlog, MaximumCount);
            if (dataSource.Any())
            {
                ItemList.DataSource = dataSource;
                ItemList.DataBind();
                PanelInteresingEntries.Visible = true;
            }
            else
            {
                PanelInteresingEntries.Visible = false;
            }
        }
    }
}