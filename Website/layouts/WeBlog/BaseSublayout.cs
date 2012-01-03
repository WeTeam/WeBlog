using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public class BaseSublayout : System.Web.UI.UserControl
    {
        /// <summary>
        /// Gets or sets the current blog
        /// </summary>
        public BlogHomeItem CurrentBlog
        {
            get;
            set;
        }

        public BaseSublayout()
        {
            CurrentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
        }
    }
}