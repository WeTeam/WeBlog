using System.Collections.Specialized;
using System.Web.Mvc;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.layouts
{
    public class RenderingParameterHelper : ParameterHelperBase<Controller>
    {
        public RenderingParameterHelper(Controller controller, bool applyProperties)
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