using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogArchive : BaseSublayout
    {
        protected DateTime m_startedDate = DateTime.Now;
        public Dictionary<int, List<EntryItem>> m_entriesByMonthAndYear = null;

        /// <summary>
        /// Gets or sets whether individual months in each year should be shown by default
        /// </summary>
        public bool ExpandMonthsOnLoad
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether individual posts in each month should be shown by default
        /// </summary>
        public bool ExpandPostsOnLoad
        {
            get;
            set;
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            ExpandMonthsOnLoad = true;
            SublayoutParamHelper helper = new SublayoutParamHelper(this, true);

            m_entriesByMonthAndYear = new Dictionary<int, List<EntryItem>>();

            if(CurrentBlog != null)
                m_startedDate = CurrentBlog.InnerItem.Statistics.Created;
            
            LoadEntries();
            if (Years != null)
            {
                Years.DataSource = GetYears();
                Years.DataBind();
            }
        }

        /// <summary>
        /// Load the entries for the blog
        /// </summary>
        protected void LoadEntries()
        {
            m_entriesByMonthAndYear = new Dictionary<int, List<EntryItem>>();

            var years = GetYears();
            foreach (var year in years)
            {
                for (int i = 1; i <= 12; i++)
                {
                    var key = (year * 100) + i;
                    m_entriesByMonthAndYear.Add(key, new List<EntryItem>());
                }
            }

            var entries = ManagerFactory.EntryManagerInstance.GetBlogEntries();
            foreach (var entry in entries)
            {
                DateTime created = entry.Created;
                var key = (created.Year * 100) + created.Month;
                if (m_entriesByMonthAndYear.ContainsKey(key))
                {
                    m_entriesByMonthAndYear[key].Add(entry);
                }
            }
        }

        /// <summary>
        /// Gets the years from the current year to the year the blog was started
        /// </summary>
        /// <returns></returns>
        protected virtual int[] GetYears()
        {
            var years = new List<int>();
            for (var year = DateTime.Now.Year; year >= m_startedDate.Year; year--)
                years.Add(year);

            return years.ToArray();
        }

        /// <summary>
        /// Get the months with entries for the given year
        /// </summary>
        /// <param name="year">The year to get the months for</param>
        /// <returns>An array of months with entries</returns>
        protected virtual int[] GetMonths(int year)
        {
            var yearKey = year * 100;

            var months = new List<int>();
            // put this in for (int i = 1; i <= 12; i++)
            //for (int i = 0; i < 12; i++)
            for (int i = 1; i <= 12; i++)
            {
                if (m_entriesByMonthAndYear.ContainsKey(yearKey + i))
                    months.Add(yearKey + i);
            }

            return months.ToArray();
        }

        /// <summary>
        /// Get the friendly name for a month index
        /// </summary>
        /// <param name="yearAndMonth">The year and month in yyyyMM format</param>
        /// <returns>The friendly month name</returns>
        protected virtual string GetFriendlyMonthName(int yearAndMonth)
        {
            if (yearAndMonth > 99999)
            {
                var month = int.Parse(yearAndMonth.ToString().Substring(4, 2));
                DateTimeFormatInfo dtft = new DateTimeFormatInfo();
                return dtft.GetMonthName(month);
            }
            else
                return "[unknown]";
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
            if (m_entriesByMonthAndYear.ContainsKey(yearAndMonth))
                return m_entriesByMonthAndYear[yearAndMonth];
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