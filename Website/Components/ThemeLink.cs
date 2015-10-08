using System.Collections.Generic;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Components
{
    public class ThemeLink : IThemeInclude
    {
        protected BlogHomeItem Blog;
        public Dictionary<HtmlTextWriterAttribute, string> Attributes { get; set; }

        public virtual bool ShouldInclude
        {
            get
            {
                return Blog != null && (!string.IsNullOrEmpty(Blog.Theme.Raw));
            }
        }

        public ThemeItem CurrentTheme
        {
            get
            {
                return new ThemeItem(Blog.Theme.Item);
            }
        }

        public ThemeLink()
        {
            Blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            Attributes = new Dictionary<HtmlTextWriterAttribute, string>
            {
                {HtmlTextWriterAttribute.Href, CurrentTheme.FileLocation.Raw.Trim()},
                {HtmlTextWriterAttribute.Rel, "stylesheet"},
                {HtmlTextWriterAttribute.Type, "text/css"}
            };
        }
    }
}