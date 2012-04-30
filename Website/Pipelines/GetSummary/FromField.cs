using System;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class FromField : GetSummaryProcessorBase
    {
        public string FieldName
        {
            get;
            set;
        }

        public FromField()
        {
            FieldName = "Introduction";
        }

        protected override void GetSummary(GetSummaryArgs args)
        {
            args.Summary = args.Entry.InnerItem[FieldName];
        }
    }
}