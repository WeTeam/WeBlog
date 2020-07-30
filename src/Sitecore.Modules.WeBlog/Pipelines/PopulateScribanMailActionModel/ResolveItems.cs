using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel
{
    /// <summary>
    /// A processor for the weBlogPopulateScribanMailActionModel pipeline which resolves the entry item to be used by other processors.
    /// </summary>
    public class ResolveItems
    {
        private IWeBlogSettings _settings = null;
        private BaseTemplateManager _templateManager = null;
        private IEntryManager _entryManager = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public ResolveItems()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="settings">The settings to use.</param>
        /// <param name="templateManager">The template manager to use to retrieve templates.</param>
        public ResolveItems(IWeBlogSettings settings, BaseTemplateManager templateManager, IEntryManager entryManager)
        {
            _settings = settings ?? WeBlogSettings.Instance;
            _templateManager = templateManager ?? ServiceLocator.ServiceProvider.GetService(typeof(BaseTemplateManager)) as BaseTemplateManager;
            _entryManager = entryManager ?? ManagerFactory.EntryManagerInstance;
        }

        public void Process(PopulateScribanMailActionModelArgs args)
        {
            EntryItem entryItem = null;
            CommentItem commentItem = null;

            var dataItem = args.WorkflowPipelineArgs.DataItem;

            if (_templateManager.TemplateIsOrBasedOn(dataItem, _settings.EntryTemplateIds))
                entryItem = new EntryItem(args.WorkflowPipelineArgs.DataItem);
            else if(_templateManager.TemplateIsOrBasedOn(dataItem, _settings.CommentTemplateIds))
            {
                commentItem = new CommentItem(args.WorkflowPipelineArgs.DataItem);
                entryItem = _entryManager.GetBlogEntryItemByCommentUri(commentItem.InnerItem.Uri);
            }

            if (args.EntryItem == null && entryItem != null)
                args.EntryItem = entryItem;

            if (args.CommentItem == null && commentItem != null)
                args.CommentItem = commentItem;
        }
    }
}