using Sitecore.Workflows;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the history for the workflow of the data item.
    /// </summary>
    public class AddWorkflowHistory
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "history";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            var dataItem = args.WorkflowPipelineArgs.DataItem;
            var workflow = dataItem.State.GetWorkflow();

            var events = new WorkflowEvent[0];

            if(workflow != null)
                events = workflow.GetHistory(dataItem);

            args.AddModel(ModelKey, events);
        }
    }
}