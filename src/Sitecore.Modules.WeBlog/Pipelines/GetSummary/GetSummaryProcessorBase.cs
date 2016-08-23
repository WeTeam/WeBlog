
namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public abstract class GetSummaryProcessorBase : IGetSummaryProcessor
    {
        public void Process(GetSummaryArgs args)
        {
            if (ShouldProcess(args))
                GetSummary(args);
        }

        protected virtual bool ShouldProcess(GetSummaryArgs args)
        {
            return string.IsNullOrEmpty(args.Summary);
        }

        protected abstract void GetSummary(GetSummaryArgs args);
    }
}