using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public abstract class CreateItem : PipelineProcessorRequest<ItemContext>
    {
        /// <summary>
        /// The <see cref="IBlogManager"/> used to locate the context blog.
        /// </summary>
        private IBlogManager _blogManager = null;

        /// <summary>
        /// The <see cref="BaseItemManager"/> used to create new items.
        /// </summary>
        private BaseItemManager _itemManager = null;

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="blogManager">The <see cref="IBlogManager"/> used to locate the context blog.</param>
        /// <param name="itemManager">The <see cref="BaseItemManager"/> used to create new items.</param>
        protected CreateItem(IBlogManager blogManager, BaseItemManager itemManager)
        {
            Assert.ArgumentNotNull(blogManager, nameof(blogManager));
            Assert.ArgumentNotNull(itemManager, nameof(itemManager));

            _blogManager = blogManager;
            _itemManager = itemManager;
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            var itemTitle = RequestContext.Argument;
            if (ItemUtil.IsItemNameValid(itemTitle ?? string.Empty))
            {
                var currentItem = RequestContext.Item;
                var currentBlog = _blogManager.GetCurrentBlog(currentItem);
                if (currentBlog != null)
                {
                    var templateId = GetTemplateId(currentBlog);
                    var parentItem = GetParentItem(currentBlog);
                    
                    Item newItem = _itemManager.AddFromTemplate(itemTitle, templateId, parentItem);

                    return new PipelineProcessorResponseValue
                    {
                        Value = new
                        {
                            itemId = newItem.ID.Guid
                        }
                    };
                }
            }
            return new PipelineProcessorResponseValue
            {
                Value = null
            };
        }

        /// <summary>
        /// Get the ID of the template to base the item on.
        /// </summary>
        /// <param name="blogItem"></param>
        /// <returns></returns>
        protected abstract ID GetTemplateId(BlogHomeItem blogItem);

        /// <summary>
        /// Get the parent item to create the new item under.
        /// </summary>
        /// <param name="blogItem">The context blog to create the item under.</param>
        /// <returns>The parent item.</returns>
        protected abstract Item GetParentItem(BlogHomeItem blogItem);
    }
}
