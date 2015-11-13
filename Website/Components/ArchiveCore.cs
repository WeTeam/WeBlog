using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Components
{
    public class ArchiveCore : IArchiveCore
    {
        protected DateTime StartedDate = DateTime.Now;
        public Dictionary<int, List<EntryItem>> EntriesByMonthAndYear { get; set; }
        public Dictionary<int, int[]> MonthsByYear { get; set; }

        public ArchiveCore(IBlogManager blogManagerInstance)
        {
            var currentBlog = blogManagerInstance.GetCurrentBlog();
            EntriesByMonthAndYear = new Dictionary<int, List<EntryItem>>();
            if (currentBlog != null)
            {
                StartedDate = currentBlog.InnerItem.Statistics.Created;
            }
            LoadEntries();
        }

        protected void LoadEntries()
        {
            var entries = ManagerFactory.EntryManagerInstance.GetBlogEntries();
            MonthsByYear = new Dictionary<int, int[]>();

            foreach (var entry in entries)
            {
                DateTime created = entry.Created;
                var key = (created.Year * 100) + created.Month;
                if (EntriesByMonthAndYear.ContainsKey(key))
                {
                    EntriesByMonthAndYear[key].Add(entry);
                }
                else
                {
                    var listTemp = new List<EntryItem> { entry };
                    EntriesByMonthAndYear.Add(key, listTemp);
                }
            }

            foreach (var blogEntryYear in GetYears(entries))
            {
                MonthsByYear.Add(blogEntryYear, GetMonths(blogEntryYear).ToArray());
            }
        }

        /// <summary>
        /// Gets the years from latest blog entry's year to oldest blog entry's year
        /// </summary>
        /// <returns></returns>
        protected virtual int[] GetYears(EntryItem[] entries)
        {
            var years = new List<int>();
            if (entries.Any())
            {
                var startYear = entries.First().Created.Year;
                var endYear = entries.Last().Created.Year;

                if (startYear != 0 && endYear != 0)
                {
                    for (var year = startYear; year >= endYear; year--)
                    {
                        if (YearHasBlogEntries(year))
                        {
                            years.Add(year);
                        }
                    }
                }
            }

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

            for (int i = 1; i <= 12; i++)
            {
                if (EntriesByMonthAndYear.ContainsKey(yearKey + i))
                {
                    months.Add(yearKey + i);
                }

            }
            return months.ToArray();
        }

        /// <summary>
        /// Get the friendly name for a month index
        /// </summary>
        /// <param name="yearAndMonth">The year and month in yyyyMM format</param>
        /// <returns>The friendly month name</returns>
        public virtual string GetFriendlyMonthName(int yearAndMonth)
        {
            if (yearAndMonth > 99999)
            {
                var month = int.Parse(yearAndMonth.ToString().Substring(4, 2));
                DateTimeFormatInfo dtft = new DateTimeFormatInfo();
                return dtft.GetMonthName(month);
            }
            return "[unknown]";
        }

        /// <summary>
        /// Check if we have blog entries for given year or not
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        protected virtual bool YearHasBlogEntries(int year)
        {
            foreach (var key in EntriesByMonthAndYear.Keys)
            {
                if (key.ToString().Contains(year.ToString()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}