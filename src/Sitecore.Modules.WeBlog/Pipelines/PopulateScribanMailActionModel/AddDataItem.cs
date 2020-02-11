namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the data item to the model.
    /// </summary>
    public class AddDataItem
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "item";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            args.AddModel(ModelKey, args.WorkflowPipelineArgs.DataItem);
        }
    }
}