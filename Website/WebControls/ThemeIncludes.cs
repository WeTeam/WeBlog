using System.Web.UI;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Web.UI;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class ThemeIncludes : WebControl
    {
        protected override void DoRender(HtmlTextWriter output)
        {
            var blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();

            if (blog != null && !string.IsNullOrEmpty(blog.Theme.Raw))
            {
                var themeItem = blog.Theme.Item;
                var currentTheme = new ThemeItem(themeItem);
                AddIncludeToOutput(output, currentTheme);
            }
        }

        protected virtual void AddIncludeToOutput(HtmlTextWriter output, ThemeItem themeItem)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Href, themeItem.FileLocation.Raw.Trim());
            output.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
            output.RenderBeginTag(HtmlTextWriterTag.Link);
            output.RenderEndTag();
        }
    }
}