using System;
using HtmlAgilityPack;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class FirstContentBlock : GetSummaryProcessorBase
    {
        /// <summary>
        /// Gets or sets the XPath expression used to find the delimiter element
        /// </summary>
        public string XPath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the field to extract the summary from
        /// </summary>
        public string FieldName
        {
            get;
            set;
        }

        public FirstContentBlock()
        {
            XPath = "//hr";
            FieldName = "Content";
        }

        protected override void GetSummary(GetSummaryArgs args)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(args.Entry[FieldName]);

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