using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Themes;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class Theme : BlogRenderingModelBase
    {        
        protected IThemeFileResolver ThemeFileResolver { get; }

        public ThemeFiles ThemeFiles { get; protected set; }

        public Theme(IThemeFileResolver themeFileResolver)
        {
            Assert.ArgumentNotNull(themeFileResolver, nameof(themeFileResolver));

            ThemeFileResolver = themeFileResolver;
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            ThemeFiles = ThemeFileResolver.Resolve(CurrentBlog.Theme.Item);
        }
    }
}