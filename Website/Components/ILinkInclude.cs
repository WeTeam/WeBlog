using System.Collections.Generic;
using System.Web.UI;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface ILinkInclude
    {
        bool ShouldInclude { get; }
        Dictionary<HtmlTextWriterAttribute, string> Attributes { get; set; }
    }
}