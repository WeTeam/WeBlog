using System;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Web.UI;

namespace Sitecore.Modules.WeBlog.WebForms.WebControls
{
    [Obsolete("Use /layouts/WeBlog/ThemeStylesheets.ascx or /layouts/WeBlog/ThemeScripts.ascx instead")]
    public class ThemeIncludes : WebControl
    {
        protected IThemeInclude ThemeLink;

        public ThemeIncludes(IThemeInclude tl = null)
        {
            ThemeLink = tl ?? new ThemeLink();
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            if (ThemeLink.ShouldInclude)
            {
                AddIncludeToOutput(output);
            }
        }

        protected virtual void AddIncludeToOutput(HtmlTextWriter output)
        {
            foreach (var a in ThemeLink.Attributes)
            {
                output.AddAttribute(a.Key, a.Value);
            }
            output.RenderBeginTag(HtmlTextWriterTag.Link);
            output.RenderEndTag();
        }
    }
}