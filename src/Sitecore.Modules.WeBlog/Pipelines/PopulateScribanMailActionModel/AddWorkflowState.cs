namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the workflow state to the model.
    /// </summary>
    public class AddWorkflowState
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "state";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            var state = args.WorkflowPipelineArgs.DataItem.State.GetWorkflowState();
            args.AddModel(ModelKey, state);
        }
    }
}