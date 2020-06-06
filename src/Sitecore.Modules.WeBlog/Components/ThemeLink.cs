using System;
using System.Collections.Generic;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Components
{
    [Obsolete("Use Sitecore.Modules.WeBlog.Themes.IThemeFileResolver from services instead.")]
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
                if (Blog != null && !string.IsNullOrEmpty(Blog.Theme.Raw))
                {
                    return new ThemeItem(Blog.Theme.Item);
                }
                return null;
            }
        }

        public ThemeLink()
        {
            Blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            Attributes = new Dictionary<HtmlTextWriterAttribute, string>
            {
                {HtmlTextWriterAttribute.Href, GetThemeUrl()},
                {HtmlTextWriterAttribute.Rel, "stylesheet"},
                {HtmlTextWriterAttribute.Type, "text/css"}
            };
        }

        protected string GetThemeUrl()
        {
            return CurrentTheme != null ? CurrentTheme.FileLocation.Raw.Trim() : string.Empty;
        }
    }
}