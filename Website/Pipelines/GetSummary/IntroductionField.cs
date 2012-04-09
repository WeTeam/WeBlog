using System;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class IntroductionField : GetSummaryProcessorBase
    {
        protected override void GetSummary(GetSummaryArgs args)
        {
            args.Summary = args.Entry.Introduction.Text;
        }
    }
}