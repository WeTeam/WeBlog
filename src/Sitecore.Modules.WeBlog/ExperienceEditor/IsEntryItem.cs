using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class IsEntryItem : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue
            {
                Value = RequestContext.Item.TemplateIsOrBasedOn(Settings.EntryTemplateID)
            };
        }

    }
}