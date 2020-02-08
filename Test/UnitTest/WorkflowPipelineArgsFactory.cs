using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Workflows.Simple;

namespace Sitecore.Modules.WeBlog.UnitTest
{
    internal static class WorkflowPipelineArgsFactory
    {
        public static WorkflowPipelineArgs CreateWorkflowPipelineArgs(Item dataItem = null)
        {
            var dataItemToUse = dataItem ?? ItemFactory.CreateItem().Object;
            var comments = new StringDictionary();
            return new WorkflowPipelineArgs(dataItemToUse, comments, new object[0]);
        }
    }
}
