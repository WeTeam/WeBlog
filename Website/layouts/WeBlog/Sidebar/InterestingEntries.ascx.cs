﻿using System;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Layouts
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

            var algororithm = Components.InterestingEntriesCore.GetAlgororithmFromString(Mode);
            if (algororithm != InterestingEntriesAlgorithm.Custom || InterestingEntriesCore == null)
            {
                InterestingEntriesCore = new InterestingEntriesCore(ManagerFactory.EntryManagerInstance, algororithm);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateList();
        }

        protected virtual void PopulateList()
        {
            ItemList.DataSource = InterestingEntriesCore.GetEntries(CurrentBlog, MaximumCount);
            ItemList.DataBind();
        }
    }
}