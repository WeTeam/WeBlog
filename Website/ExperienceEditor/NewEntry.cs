using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class NewEntry : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            var itemTitle = RequestContext.Argument;
            var currentItem = RequestContext.Item;
            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(currentItem);
            if (currentBlog != null)
            {
                var template = new TemplateID(currentBlog.BlogSettings.EntryTemplateID);
                Item newItem = ItemManager.AddFromTemplate(itemTitle, template, currentBlog);

                ContentHelper.PublishItem(newItem);

                return new PipelineProcessorResponseValue
                {
                    Value = newItem.ID.Guid
                };
            }
            return new PipelineProcessorResponseValue
            {
                Value = null
            };
        }
    }
}