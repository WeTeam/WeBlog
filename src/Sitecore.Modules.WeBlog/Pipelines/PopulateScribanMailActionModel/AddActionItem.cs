namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the workflow action definition item to the model.
    /// </summary>
    public class AddActionItem
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "actionItem";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            args.AddModel(ModelKey, args.WorkflowPipelineArgs.ProcessorItem);
        }
    }
}