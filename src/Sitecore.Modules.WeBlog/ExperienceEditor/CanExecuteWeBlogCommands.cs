using Sitecore.Modules.WeBlog.Managers;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class CanExecuteWeBlogCommands : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            var currentItem = RequestContext.Item;
            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(currentItem);
            return new PipelineProcessorResponseValue
            {
                Value = currentBlog != null
            };
        }
    }
}