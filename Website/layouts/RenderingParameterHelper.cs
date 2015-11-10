using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.layouts
{
    public class RenderingParameterHelper<T> : ParameterHelperBase<T>
    {
        public RenderingParameterHelper(T controller, bool applyProperties)
        {
            var renderingContext = RenderingContext.CurrentOrNull;
            if (renderingContext != null)
            {
                Parameters = Web.WebUtil.ParseUrlParameters(renderingContext.Rendering.Properties["Parameters"]);
                if (applyProperties)
                {
                    ApplyParameters(controller);
                }
            }
        }
    }
}