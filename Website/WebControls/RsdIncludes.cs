using System.Web.UI;
using Sitecore.Modules.WeBlog.Components.RsdLink;
using Sitecore.Web.UI;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class RsdIncludes : WebControl
    {
        protected IRsdInclude RsdLink;

        public RsdIncludes(IRsdInclude rl = null)
        {
            RsdLink = rl ?? new RsdLink();
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            if (RsdLink.ShouldInclude)
            {
                AddLinkToOutput(output);
            }
        }

        protected virtual void AddLinkToOutput(HtmlTextWriter output)
        {
            foreach (var a in RsdLink.Attributes)
            {
                output.AddAttribute(a.Key, a.Value);
            }
            output.RenderBeginTag(HtmlTextWriterTag.Link);
            output.RenderEndTag();
        }
    }
}