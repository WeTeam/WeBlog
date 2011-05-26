using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using Sitecore.Links;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Web;

namespace Sitecore.Modules.Blog.WebControls
{
    public class Tagcloud : Sitecore.Web.UI.WebControls.FieldControl
    {
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            if (TagManager.GetAllTags().Count != 0)
            {
                // Get url of parent 
                String browseUrl = LinkManager.GetItemUrl(BlogManager.GetCurrentBlog().InnerItem);
                browseUrl = WebUtil.RemoveQueryString(browseUrl);

                string CloudLiteral = GetCloud(GetCloudData(), "<a class='weight$weight$' href='" + browseUrl + "?tag=$urlencodetag$'>$tag$</a> ");
                
                output.Write("<div class=\"tagcloud\">");
                output.AddAttribute(HtmlTextWriterAttribute.Class, "tagcloud");
                output.Write(CloudLiteral);
                output.Write("</div>");
            }
        }

        protected static DataTable GetCloudData()
        {
            var tagData = new DataTable("tags");
            tagData.Columns.Add(new DataColumn("tag"));
            tagData.Columns.Add(new DataColumn("count", typeof(Int32)));
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
                double weightPercent = (double.Parse(row["count"].ToString()) / max) * 100;
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
    }
}
