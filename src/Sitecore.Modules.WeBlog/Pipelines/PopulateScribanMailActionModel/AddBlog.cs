using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which adds the blog item to the model.
    /// </summary>
    public class AddBlog
    {
        /// <summary>
        /// The key used to identify the model.
        /// </summary>
        public const string ModelKey = "blog";

        private IBlogManager _blogManager = null;

        public AddBlog()
            : this(null)
        {
        }

        public AddBlog(IBlogManager blogManager)
        {
            _blogManager = blogManager ?? ManagerFactory.BlogManagerInstance;
        }

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            var blog = _blogManager.GetCurrentBlog(args.WorkflowPipelineArgs.DataItem);

            if (blog != null)
                args.AddModel(ModelKey, blog);
        }
    }
}