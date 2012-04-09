using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class AutoGenerate : GetSummaryProcessorBase
    {
        public int MaximumCharacterCount
        {
            get;
            set;
        }

        public bool StripTags
        {
            get;
            set;
        }

        public string MoreString
        {
            get;
            set;
        }

        public AutoGenerate()
        {
            MaximumCharacterCount = 500;
            StripTags = true;
            MoreString = "...";
        }

        protected override void GetSummary(GetSummaryArgs args)
        {
            if (args.Entry == null)
                return;

            var content = args.Entry.Content.Text;
            if(StripTags)
                content = StringUtil.RemoveTags(content);

            if (content.Length <= GetMaximumCharacterCount())
            {
                args.Summary = content;
            }
            else
            {
                if (StripTags)
                {
                    args.Summary = content.Substring(0, GetMaximumCharacterCount()) + GetMoreString();
                }
                else
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(content);

                    var count = 0;
                    var limitIndex = FindLimitIndex(doc.DocumentNode, ref count, GetMaximumCharacterCount());

                    if (limitIndex < content.Length)
                    {
                        var trimmedDoc = new HtmlDocument();

                        trimmedDoc.LoadHtml(content.Substring(0, limitIndex) + GetMoreString());
                        args.Summary = trimmedDoc.DocumentNode.OuterHtml;
                    }
                    else
                        args.Summary = content;
                }
            }
        }

        protected virtual int GetMaximumCharacterCount()
        {
            return MaximumCharacterCount;
        }

        protected virtual string GetMoreString()
        {
            return MoreString;
        }

        protected virtual int FindLimitIndex(HtmlNode currentNode, ref int currentCount, int maxCount)
        {
            if (currentNode.NodeType == HtmlNodeType.Text)
            {
                var prevCount = currentCount;
                currentCount += currentNode.InnerText.Length;

                if (currentCount >= maxCount)
                {
                    var extraChars = maxCount - prevCount;
                    return currentNode.StreamPosition + extraChars;
                }
            }

            if (currentNode.HasChildNodes)
            {
                foreach (var node in currentNode.ChildNodes)
                {
                    var index = FindLimitIndex(node, ref currentCount, maxCount);
                    if (index != -1)
                        return index;
                }
            }

            return -1;
        }
    }
}