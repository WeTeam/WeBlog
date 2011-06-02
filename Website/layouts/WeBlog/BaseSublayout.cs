using Sitecore.Modules.WeBlog.Items.Blog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public class BaseSublayout : System.Web.UI.UserControl
    {
        /// <summary>
        /// Gets or sets the current blog
        /// </summary>
        public BlogItem CurrentBlog
        {
            get;
            set;
        }

        public BaseSublayout()
        {
            CurrentBlog = BlogManager.GetCurrentBlog();
        }
    }
}