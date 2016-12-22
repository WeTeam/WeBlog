using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class NewCategory : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            var itemTitle = RequestContext.Argument;
            if (ItemUtil.IsItemNameValid(itemTitle))
            {
                var currentItem = RequestContext.Item;
                var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(currentItem);
                if (currentBlog != null)
                {
                    var template = new TemplateID(currentBlog.BlogSettings.CategoryTemplateID);
                    var categories = ManagerFactory.CategoryManagerInstance.GetCategoryRoot(currentItem);
                    var newItem = ItemManager.AddFromTemplate(itemTitle, template, categories);

                    return new PipelineProcessorResponseValue
                    {
                        Value = newItem.ID.Guid
                    };
                }
            }
            return new PipelineProcessorResponseValue
            {
                Value = null
            };
        }
    }
}