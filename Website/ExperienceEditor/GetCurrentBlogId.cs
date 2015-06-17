using Sitecore.Modules.WeBlog.Managers;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class GetCurrentBlogId : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue
            {
                Value = ManagerFactory.BlogManagerInstance.GetCurrentBlog(RequestContext.Item).ID.Guid
            };
        }
    }
}