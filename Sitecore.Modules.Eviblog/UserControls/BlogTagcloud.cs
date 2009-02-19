using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Web;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogTagcloud : UserControl
    {
        #region Fields

        protected Literal CloudLiteral;
        protected Sitecore.Web.UI.WebControls.Text titleTagcloud;
        protected Panel PanelTagCloud;

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (TagManager.GetAllTags().Count == 0)
            {
                PanelTagCloud.Visible = false;
            }
            else
            {
                // Set title
                titleTagcloud.Item = BlogManager.GetCurrentBlogItem();
                // Get url of parent 
                String browseUrl = LinkManager.GetItemUrl(BlogManager.GetCurrentBlogItem());
                browseUrl = WebUtil.RemoveQueryString(browseUrl);

                CloudLiteral.Text = GetCloud(GetCloudData(),"<a class='weight$weight$' href='" + browseUrl +"?tag=$urlencodetag$'>$tag$</a> ");
            }
        }

        protected static DataTable GetCloudData()
        {
            var tagData = new DataTable("tags");
            tagData.Columns.Add(new DataColumn("tag"));
            tagData.Columns.Add(new DataColumn("count", typeof (Int32)));
            DataRow newRow;

            Dictionary<string, int> tags = TagManager.GetAllTags();

            foreach (string tag in tags.Keys)
            {
                int count = tags[tag];

                newRow = tagData.Rows.Add();
                newRow["tag"] = tag;
                newRow["count"] = count;
            }

            return tagData;
        }

        private static string GetCloud(DataTable tagData, string cloudTemplate)
        {
            if (!tagData.Columns.Contains("tag"))
                throw new Exception("Expected column 'tag' is missing");
            if (!tagData.Columns.Contains("count"))
                throw new Exception("Expected column 'count' is missing");

            StringBuilder outputBuffer = new StringBuilder();
            double max = 0;
            double min = 0;

            double.TryParse(tagData.Compute("min(count)", null).ToString(), out min);
            double.TryParse(tagData.Compute("max(count)", null).ToString(), out max);

            // Loop through the data, generate the tag cloud
            foreach (DataRow row in tagData.Rows)
            {
                // Calculate the weight
                double weightPercent = (double.Parse(row["count"].ToString())/max)*100;
                int weight;

                // Give weigth to tags
                weight = GetWeight(weightPercent);

                // Only add the tags with an weight higher then 0
                if (weight > 0)
                {
                    string txt = cloudTemplate;
                    txt = txt.Replace("$weight$", weight.ToString());
                    txt = txt.Replace("$tag$", row["tag"].ToString());
                    txt = txt.Replace("$urlencodetag$", HttpUtility.UrlEncode(row["tag"].ToString()));

                    // Add to tagcloud
                    outputBuffer.Append(txt);
                }
            }

            return outputBuffer.ToString();
        }

        private static int GetWeight(double weightPercent)
        {
            var weight = weightPercent >= 99 ? 1 : (weightPercent >= 70 ? 2 : (weightPercent >= 40 ? 3 : (weightPercent >= 20 ? 4 : (weightPercent >= 3 ? 5 : 0))));
            return weight;
        }

        #endregion
    }
}