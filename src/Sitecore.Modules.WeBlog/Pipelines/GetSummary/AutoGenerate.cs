using HtmlAgilityPack;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class AutoGenerate : GetSummaryProcessorBase
    {
        /// <summary>
        /// Gets or sets the maximum number of characters in the generated summary
        /// </summary>
        public int MaximumCharacterCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether tags should be stripped or preserved
        /// </summary>
        public bool StripTags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text to append to the end of the summary if the field is truncated
        /// </summary>
        public string MoreString
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

        public AutoGenerate()
        {
            MaximumCharacterCount = 500;
            StripTags = true;
            MoreString = "...";
            FieldName = "Content";
        }

        protected override void GetSummary(GetSummaryArgs args)
        {
            if (args.Entry == null)
            {
                args.Summary = string.Empty;
                return;
            }

            var content = args.Entry[FieldName];
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