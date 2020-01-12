using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the next workflow state to the model.
    /// </summary>
    public class AddNextWorkflowState
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "nextState";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            var nextStateItem = GetNextStateItem(args.WorkflowPipelineArgs.ProcessorItem.InnerItem);
            args.AddModel(ModelKey, nextStateItem == null ? string.Empty : nextStateItem.Name);
        }

        protected Item GetNextStateItem(Item actionItem)
        {
            var commandItem = actionItem.Parent;
            var nextStateID = commandItem[FieldIDs.NextState];

            if (string.IsNullOrWhiteSpace(nextStateID))
                return null;

            var nextStateItem = commandItem.Database.GetItem(nextStateID);
            return nextStateItem;
        }
    }
}