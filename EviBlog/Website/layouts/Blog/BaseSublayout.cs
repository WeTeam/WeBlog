using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Modules.Blog.Items.Blog;

namespace Sitecore.Modules.Blog.Layouts
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