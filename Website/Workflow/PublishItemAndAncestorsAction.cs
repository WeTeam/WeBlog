using Sitecore.Workflows.Simple;

namespace Sitecore.Modules.WeBlog.Workflow
{
    public class PublishItemAndAncestorsAction
    {
        public void Process(WorkflowPipelineArgs args)
        {
            if(args.DataItem != null)
                ContentHelper.PublishItemAndRequiredAncestors(args.DataItem);
        }
    }
}