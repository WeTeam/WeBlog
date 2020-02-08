using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the comment item to the model.
    /// </summary>
    public class AddComment
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "comment";

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            if (args.CommentItem == null)
                return;

            var entryItem = new CommentItem(args.WorkflowPipelineArgs.DataItem);
            args.AddModel(ModelKey, entryItem);
        }
    }
}