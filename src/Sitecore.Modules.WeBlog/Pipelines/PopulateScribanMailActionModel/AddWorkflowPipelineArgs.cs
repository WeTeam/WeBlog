namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the WorkflowPipelineArgs to the model.
    /// </summary>
    public class AddWorkflowPipelineArgs
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "args";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            args.AddModel(ModelKey, args.WorkflowPipelineArgs);
        }
    }
}