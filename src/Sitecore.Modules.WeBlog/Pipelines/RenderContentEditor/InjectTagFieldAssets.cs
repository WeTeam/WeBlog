using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.StringExtensions;
using Sitecore.Xml;

namespace Sitecore.Modules.WeBlog.Pipelines.RenderContentEditor
{
    public class InjectTagFieldAssets
    {
        private static readonly List<string> IncludedScripts = new List<string>();
        private static readonly List<string> IncludedStyles = new List<string>();

        public void AddScript(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");
            string src = XmlUtil.GetAttribute("src", configNode);
            IncludedScripts.Add(src);
        }

        public void AddStylesheet(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");
            string src = XmlUtil.GetAttribute("src", configNode);
            IncludedStyles.Add(src);
        }

        public void Process(PipelineArgs args)
        {
            IncludeAsset(IncludedScripts, "<script src=\"{0}\"></script>");
            IncludeAsset(IncludedStyles, "<link href=\"{0}\" rel=\"stylesheet\" />");
        }

        private void IncludeAsset(List<string> assets, string resourceTag)
        {
            foreach (var resource in assets)
            {
                Context.Page.Page.Header.Controls.Add(new LiteralControl(resourceTag.FormatWith(resource)));
            }
        }
    }
}