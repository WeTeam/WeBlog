using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;
using System;

namespace Sitecore.Modules.WeBlog.ExperienceEditor
{
    public class IsEntryItem : PipelineProcessorRequest<ItemContext>
    {
        private IWeBlogSettings _settings = null;

        private BaseTemplateManager _templateManager = null;

        public IsEntryItem()
            : this(WeBlogSettings.Instance, ServiceLocator.ServiceProvider.GetService(typeof(BaseTemplateManager)) as BaseTemplateManager)
        {
        }
        
        [Obsolete("Use ctor(IWeBlogSettings, BaseTemplateManager) instead.")]
        public IsEntryItem(IWeBlogSettings settings)
            : this(settings, ServiceLocator.ServiceProvider.GetService(typeof(BaseTemplateManager)) as BaseTemplateManager)
        {
        }

        public IsEntryItem(IWeBlogSettings settings, BaseTemplateManager templateManager)
        {
            Assert.ArgumentNotNull(templateManager, nameof(templateManager));
            _settings = settings;
            _templateManager = templateManager;
        }

        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue
            {
                Value = RequestContext.Item.TemplateIsOrBasedOn(_templateManager, _settings.EntryTemplateIds)
            };
        }

    }
}