using System;
using System.Linq;
using System.Web;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Web;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogTagCloud : BaseSublayout
    {
        private int m_min = int.MaxValue;
        private int m_max = int.MinValue;

        protected void Page_Load(object sender, EventArgs e)
        {
             LoadTags();
        }

        /// <summary>
        /// Load the tags for the current blog into this control
        /// </summary>
        protected virtual void LoadTags()
        {
            if (ManagerFactory.TagManagerInstance.GetAllTags().Count == 0)
            {
                if(PanelTagCloud != null)
                    PanelTagCloud.Visible = false;
            }
            else
            {
                var tags = ManagerFactory.TagManagerInstance.GetAllTags();

                m_min = (from tag in tags select tag.Value).Min();
                m_max = (from tag in tags select tag.Value).Max();

                if (TagList != null)
                {
                    TagList.DataSource = tags;
                    TagList.DataBind();
                }
            }
        }

        /// <summary>
        /// Calculate the weight class to use for the current tag
        /// </summary>
        /// <param name="tagWeight">The weight to get the weight class for</param>
        /// <returns>The weight class for the given weight</returns>
        protected virtual string GetTagWeightClass(int tagWeight)
        {
            var weightPercent = ((double)tagWeight / m_max) * 100;

            // Give weigth to tags
            return GetWeight(weightPercent).ToString();
        }

        /// <summary>
        /// Get the URL for a tag
        /// </summary>
        /// <param name="tag">The tag to get the URL for</param>
        /// <returns>The tag URL</returns>
        protected virtual string GetTagUrl(string tag)
        {
            // Get url of parent 
            var browseUrl = WebUtil.RemoveQueryString(LinkManager.GetItemUrl(CurrentBlog.InnerItem));

            return browseUrl + "?tag=" + HttpUtility.UrlEncode(tag);
        }

        /// <summary>
        /// Calculate the weight percentile for this tag
        /// </summary>
        /// <param name="weightPercent">The weight as a percentage</param>
        /// <returns>The weight percentile</returns>
        protected int GetWeight(double weightPercent)
        {
            var weight = weightPercent >= 99 ? 1 : (weightPercent >= 70 ? 2 : (weightPercent >= 40 ? 3 : (weightPercent >= 20 ? 4 : (weightPercent >= 3 ? 5 : 0))));
            return weight;
        }
    }
}