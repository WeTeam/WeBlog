using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Converters;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogArchive : BaseSublayout
    {
        public IArchiveCore ArchiveCore { get; set; }

        public BlogArchive(IArchiveCore archiveCore = null)
        {
            ArchiveCore = archiveCore ?? new ArchiveCore(Managers.ManagerFactory.BlogManagerInstance);
        }

        /// <summary>
        /// Gets or sets whether individual months in each year should be shown by default
        /// </summary>
        [TypeConverter(typeof(ExtendedBooleanConverter))]
        public bool ExpandMonthsOnLoad
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether individual posts in each month should be shown by default
        /// </summary>
        [TypeConverter(typeof(ExtendedBooleanConverter))]
        public bool ExpandPostsOnLoad
        {
            get;
            set;
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            ExpandMonthsOnLoad = true;
            Years.DataSource = ArchiveCore.MonthsByYear.Keys;
            Years.DataBind();
        }

        protected virtual int[] GetMonths(int year)
        {
            return ArchiveCore.MonthsByYear[year];
        }

        protected virtual string GetFriendlyMonthName(int yearAndMonth)
        {
            return ArchiveCore.GetFriendlyMonthName(yearAndMonth);
        }

        /// <summary>
        /// Get the count of entries for the given year and month
        /// </summary>
        /// <param name="yearAndMonth">The year and month in yyyyMM format</param>
        /// <returns>The number of entries for the given year and month</returns>
        protected virtual int GetEntryCountForYearAndMonth(int yearAndMonth)
        {
            return GetEntriesForYearAndMonth(yearAndMonth).Count;
        }

        /// <summary>
        /// Get the entries for the given year and month
        /// </summary>
        /// <param name="yearAndMonth">The year and month in yyyyMM format</param>
        /// <returns>The entries for the given year and month</returns>
        protected virtual List<EntryItem> GetEntriesForYearAndMonth(int yearAndMonth)
        {
            if (ArchiveCore.EntriesByMonthAndYear.ContainsKey(yearAndMonth))
                return ArchiveCore.EntriesByMonthAndYear[yearAndMonth];
            else
                return new List<EntryItem>();
        }

        protected virtual void MonthDataBound(object sender, RepeaterItemEventArgs args)
        {
            if (args.Item.ItemType == ListItemType.AlternatingItem || args.Item.ItemType == ListItemType.Item)
            {
                if (GetEntryCountForYearAndMonth((int)args.Item.DataItem) <= 0)
                    args.Item.Controls.Clear();
            }
        }
    }
}