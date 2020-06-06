using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Themes;
using Sitecore.Mvc.Presentation;
using System;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class Theme : BlogRenderingModelBase
    {        
        [Obsolete("Use the ThemeFileResolver property instead.")]
        public ThemeItem ThemeItem { get; protected set; }

        protected IThemeFileResolver ThemeFileResolver { get; }

        public ThemeFiles ThemeFiles { get; protected set; }

        public Theme(IThemeFileResolver themeFileResolver)
        {
            ThemeFileResolver = themeFileResolver;
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);

            ThemeItem = CurrentBlog.Theme.Item;
            ThemeFiles = ThemeFileResolver.Resolve(CurrentBlog.Theme.Item);
        }
    }
}