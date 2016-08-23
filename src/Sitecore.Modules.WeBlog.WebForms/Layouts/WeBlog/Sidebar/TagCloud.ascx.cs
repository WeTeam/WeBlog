using System;
using System.Linq;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar
{
    public partial class BlogTagCloud : BaseSublayout
    {
        protected ITagCloudCore TagCloudCore { get; set; }

        public int MaximumCount { get; set; }

        public string SortingOptions { get; set; }

        public BlogTagCloud(ITagCloudCore tagCloudCore = null)
        {
            TagCloudCore = tagCloudCore ?? new TagCloudCore(ManagerFactory.BlogManagerInstance);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadTags();
            if (TagSortList != null)
            {
                TagSortList.DataSource = TagCloudCore.GetSortNames(SortingOptions);
                TagSortList.DataBind();
            }

        }

        /// <summary>
        /// Load the tags for the current blog into this control
        /// </summary>
        protected virtual void LoadTags()
        {
            if (!TagCloudCore.Tags.Any() || MaximumCount <= 0)
            {
                if (PanelTagCloud != null)
                    PanelTagCloud.Visible = false;
            }
            else
            {
                if (TagList != null)
                {
                    TagList.DataSource = TagCloudCore.Tags.Take(MaximumCount);
                    TagList.DataBind();
                }
            }
        }
    }
}