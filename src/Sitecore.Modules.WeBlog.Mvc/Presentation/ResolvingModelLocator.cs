using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Presentation
{
    /// <summary>
    /// A model locator which supports dependency injection into model instances.
    /// </summary>
    public class ResolvingModelLocator : ModelLocator
    {
        protected override object GetModelFromTypeName(string typeName, string model, bool throwOnTypeCreationError)
        {
            var type = TypeHelper.GetType(typeName);
            var instance = ActivatorUtilities.CreateInstance(ServiceLocator.ServiceProvider, type);
            return instance;
        }
    }
}
