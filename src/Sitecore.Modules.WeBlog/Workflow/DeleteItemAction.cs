using Sitecore.Workflows.Simple;

namespace Sitecore.Modules.WeBlog.Workflow
{
    public class DeleteItemAction
    {
        public void Process(WorkflowPipelineArgs args)
        {
            var item = args.DataItem;
            if (item != null)
            {
                item.Delete();
            }
        }
    }
}