using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Blog.Managers;

namespace Sitecore.Modules.Blog.WebControls
{
    public class BlogArchive : System.Web.UI.WebControls.WebControl
    {
        public List<int> listYears = new List<int>();
        public List<int> listMonths = new List<int>();
        public PlaceHolder phYears = new PlaceHolder();
        public DateTime startedDate = BlogManager.GetCurrentBlog().InnerItem.Statistics.Created;

        protected override void CreateChildControls()
        {
            Controls.Add(phYears);

            GetYears();
            
            base.CreateChildControls();
        }

        private void AddListItem(Control control)
        {
            phYears.Controls.Add(new LiteralControl("<li>"));
            phYears.Controls.Add(control);
            phYears.Controls.Add(new LiteralControl("</li>"));
        }

        private void GetYears()
        {
            int year = startedDate.Year;

            if (year != DateTime.Now.Year)
            {
                while (year <= DateTime.Now.Year)
                {
                    HyperLink link = new HyperLink();
                    link.Text = year.ToString();
                    //link.Attributes.Add("onClick", "ToggleVisibility(\"month-" + year + "\")");
                    link.Attributes.Add("class", "year");

                    AddListItem(link);

                    GetMonths(year);

                    year = year + 1;
                }
            }
            else
            {
                GetMonths(year);
            }
        }

        private void GetMonths(int year)
        {
            int currentYear = year;
            int month;
            int maxMonths;

            if (currentYear == startedDate.Year)
                month = startedDate.Month;
            else
                month = 1;

            if (currentYear == DateTime.Now.Year)
                maxMonths = DateTime.Now.Month;
            else
                maxMonths = 12;

            if (month <= 12)
            {
                phYears.Controls.Add(new LiteralControl("<ul id=\"month-" + year + "\" class=\"month\">"));

                while (month <= maxMonths)
                {
                    DateTimeFormatInfo dtft = new DateTimeFormatInfo();
                    string monthName = dtft.GetMonthName(month);
                    var listEntries = EntryManager.GetBlogEntriesByMonthAndYear(month, currentYear);

                    HyperLink link = new HyperLink();
                    link.Text = monthName + " (" + listEntries.Length.ToString() + ")";
                    //link.Attributes.Add("onClick", "ToggleVisibility(\"entries-" + year + month + "\")");
                    link.Attributes.Add("class", "month");

                    AddListItem(link);

                    phYears.Controls.Add(new LiteralControl("<ul id=\"entries-" + year + month + "\" class=\"entries\">"));
                    foreach (Sitecore.Modules.Blog.Items.Blog.EntryItem entry in listEntries)
                    {
                        HyperLink entryLink = new HyperLink();
                        entryLink.Text = entry.Title.Text;
                        entryLink.NavigateUrl = entry.Url;

                        AddListItem(entryLink);
                    }
                    phYears.Controls.Add(new LiteralControl("</ul>"));
                    month++;
                }
                phYears.Controls.Add(new LiteralControl("</ul>"));
            }

        }
    }
}
