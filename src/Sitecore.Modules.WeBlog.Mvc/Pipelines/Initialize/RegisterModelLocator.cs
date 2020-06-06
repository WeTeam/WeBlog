using Sitecore.Modules.WeBlog.Mvc.Presentation;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Presentation;
using Sitecore.Pipelines;

namespace Sitecore.Modules.WeBlog.Mvc.Pipelines.Initialize
{
    public class RegisterModelLocator
    {
        public void Process(PipelineArgs args)
        {
            MvcSettings.RegisterObject<ModelLocator>(() => new ResolvingModelLocator());
        }
    }
}
