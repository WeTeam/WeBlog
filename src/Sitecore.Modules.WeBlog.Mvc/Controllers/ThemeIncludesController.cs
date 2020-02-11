using Sitecore.Modules.WeBlog.Components;
using System;

namespace Sitecore.Modules.WeBlog.Mvc.Controllers
{
    [Obsolete("Use /Views/WeBlog/ThemeScripts.cshtml or /Views/WeBlog/ThemeStylesheets.cshtml instead.")]
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