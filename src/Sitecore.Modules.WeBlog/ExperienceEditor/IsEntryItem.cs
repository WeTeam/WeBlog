using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class IsEntryItem : PipelineProcessorRequest<ItemContext>
    {
        private IWeBlogSettings _settings;

        public IsEntryItem()
            : this(WeBlogSettings.Instance)
        {
        }

        public IsEntryItem(IWeBlogSettings settings)
        {
            _settings = settings;
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue
            {
                Value = RequestContext.Item.TemplateIsOrBasedOn(_settings.EntryTemplateIds)
            };
        }

    }
}