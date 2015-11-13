using System;
using System.Linq;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogTagCloud : BaseSublayout
    {
        protected ITagCloudCore TagCloudCore { get; set; }

        public BlogTagCloud(ITagCloudCore tagCloudCore = null)
        {
            TagCloudCore = tagCloudCore ?? new TagCloudCore(ManagerFactory.BlogManagerInstance);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadTags();
        }

        /// <summary>
        /// Load the tags for the current blog into this control
        /// </summary>
        protected virtual void LoadTags()
        {
            if (!TagCloudCore.Tags.Any())
            {
                if (PanelTagCloud != null)
                    PanelTagCloud.Visible = false;
            }
            else
            {
                if (TagList != null)
                {
                    TagList.DataSource = TagCloudCore.Tags;
                    TagList.DataBind();
                }
            }
        }
    }
}