using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Data.Items;
using System.Globalization;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public partial class BlogArchive : System.Web.UI.UserControl
    {
        protected System.Web.UI.WebControls.PlaceHolder phYears;

        public List<int> listYears = new List<int>();
        public List<int> listMonths = new List<int>();
        public DateTime startedDate = BlogManager.GetCurrentBlogItem().Statistics.Created;

		/// <summary>
		/// Gets or sets whether individual posts in each month should be shown
		/// </summary>
		public bool ExpandPostsOnLoad
		{
			get;
			set;
		}
            
        protected void Page_Load(object sender, EventArgs e)
        {
			Utilities.Presentation.SetProperties(this);
            GetYears();
        }

        private void AddListItem(Control control)
        {
            phYears.Controls.Add(new LiteralControl("<li>"));
            phYears.Controls.Add(control);
            phYears.Controls.Add(new LiteralControl("</li>"));
        }

        private void GetYears()
        {
			int year = DateTime.Now.Year;

			if (year != startedDate.Year)
            {
				while (year >= startedDate.Year)
                {
                    HyperLink link = new HyperLink();
                    link.Text = year.ToString();
                    link.Attributes.Add("onClick", "ToggleVisibility(\"month-" + year + "\")");
                    link.Attributes.Add("class", "year");

                    AddListItem(link);

                    GetMonths(year);

					year--;
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

            if(currentYear == DateTime.Now.Year)
                maxMonths = DateTime.Now.Month;
            else
                maxMonths = 12;

            if(month<= 12)
            {
                phYears.Controls.Add(new LiteralControl("<ul id=\"month-" + year + "\" class=\"month\">"));

                while (month <= maxMonths)
                {
                    DateTimeFormatInfo dtft = new DateTimeFormatInfo();
                    string monthName = dtft.GetMonthName(month);
                    List<Entry> listEntries = EntryManager.GetBlogEntriesByMonthAndYear(month, currentYear);

					if (listEntries.Count > 0)
					{
						HyperLink link = new HyperLink();
						link.Text = monthName + " (" + listEntries.Count.ToString() + ")";
						link.Attributes.Add("onClick", "ToggleVisibility(\"entries-" + year + month + "\")");
						link.Attributes.Add("class", "month");

						phYears.Controls.Add(new LiteralControl("<li>"));
						phYears.Controls.Add(link);

						var entriesList = "<ul id=\"entries-" + year + month + "\" class=\"entries\"";

						if (!ExpandPostsOnLoad)
							entriesList += " style=\"display:none;\"";

						entriesList += ">";

						phYears.Controls.Add(new LiteralControl(entriesList));
						foreach (Entry entry in listEntries)
						{
							HyperLink entryLink = new HyperLink();
							entryLink.Text = entry.Title;
							entryLink.NavigateUrl = entry.Url;

							AddListItem(entryLink);
						}
						phYears.Controls.Add(new LiteralControl("</ul>"));

						phYears.Controls.Add(new LiteralControl("</li>"));
					}
                    month++;
                }
                phYears.Controls.Add(new LiteralControl("</ul>"));
            }
            
        }
    }
}