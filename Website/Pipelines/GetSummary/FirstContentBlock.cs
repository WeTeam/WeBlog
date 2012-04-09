using System;
using HtmlAgilityPack;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class FirstContentBlock : GetSummaryProcessorBase
    {
        public string XPath
        {
            get;
            set;
        }

        public FirstContentBlock()
        {
            XPath = "//hr";
        }

        protected override void GetSummary(GetSummaryArgs args)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(args.Entry.Content.Text);

            var blockLimiter = doc.DocumentNode.SelectSingleNode(GetXPath());
            if (blockLimiter != null)
            {
                var index = blockLimiter.StreamPosition;
                var content = doc.DocumentNode.OuterHtml;

                if (index < content.Length)
                {
                    var trimmedDoc = new HtmlDocument();

                    trimmedDoc.LoadHtml(content.Substring(0, index));
                    args.Summary = trimmedDoc.DocumentNode.OuterHtml;
                }
                else
                    args.Summary = content;
            }
        }

        protected virtual string GetXPath()
        {
            return XPath;
        }
    }
}