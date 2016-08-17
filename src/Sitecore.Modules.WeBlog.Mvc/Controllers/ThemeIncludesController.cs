using Sitecore.Modules.WeBlog.Components;

namespace Sitecore.Modules.WeBlog.Mvc.Controllers
{
    public class ThemeIncludesController : LinkIncludeController
    {
        protected IThemeInclude ThemeLink;

        public ThemeIncludesController() : this(new ThemeLink()) { }

        public ThemeIncludesController(IThemeInclude tl = null) : base(tl)
        {
            ThemeLink = tl ?? new ThemeLink();
        }
    }
}