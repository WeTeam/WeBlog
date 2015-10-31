using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public abstract class LinkIncludeController : Controller
    {
        protected ILinkInclude Link;

        protected LinkIncludeController(ILinkInclude li)
        {
            Link = li;
        }

        public virtual void Index()
        {
            if (Link.ShouldInclude)
            {
                AddLinkToOutput();
            }
        }

        protected virtual void AddLinkToOutput()
        {
            var include = new Tag(HtmlTextWriterTag.Link.ToString());
            foreach (var a in Link.Attributes)
            {
                include.Attributes.Add(a.Key.ToString(), a.Value);
            }
            HttpContext.Response.Write(include.Start());
        }
    }
}