using System;
using System.Linq;
using Sitecore.Modules.WeBlog.Components.RecentComments;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogRecentComments : BaseSublayout
    {
        public IRecentCommentsCore RecentCommentsCore { get; set; }

        public BlogRecentComments(IRecentCommentsCore recentCommentsCore = null)
        {
            RecentCommentsCore = recentCommentsCore ?? new RecentCommentsCore(ManagerFactory.BlogManagerInstance);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!RecentCommentsCore.Comments.Any())
            {
                if (PanelRecentComments != null)
                    PanelRecentComments.Visible = false;
            }
            else
            {
                if (ListViewRecentComments != null)
                {
                    ListViewRecentComments.DataSource = RecentCommentsCore.Comments;
                    ListViewRecentComments.DataBind();
                }
            }
        }
    }
}