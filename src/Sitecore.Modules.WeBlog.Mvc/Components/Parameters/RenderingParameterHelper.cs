using Sitecore.Modules.WeBlog.Components.Parameters;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Components.Parameters
{
    public class RenderingParameterHelper<T> : ParameterHelperBase<T>
    {
        public RenderingParameterHelper(T controller, bool applyProperties)
        {
            var renderingContext = RenderingContext.CurrentOrNull;
            if (renderingContext != null)
            {
                var rawParameters = renderingContext.Rendering.Properties["Parameters"];

                if (!string.IsNullOrEmpty(rawParameters))
                {
                    Parameters = Web.WebUtil.ParseUrlParameters(rawParameters);
                    if (applyProperties)
                    {
                        ApplyParameters(controller);
                    }
                }
            }
        }
    }
}