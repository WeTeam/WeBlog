using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Themes;
using System;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    [AllowDependencyInjection]
    public partial class ThemeScripts : BaseSublayout
    {
        protected IThemeFileResolver ThemeFileResolver { get; }

        [Obsolete("Use the ThemeFiles property instead.")]
        protected ThemeItem ThemeItem
        {
            get
            {
                return CurrentBlog.Theme.Item;
            }
        }

        protected ThemeFiles ThemeFiles { get; set; }

        public ThemeScripts(IThemeFileResolver themeFileResolver)
        {
            ThemeFileResolver = themeFileResolver;
        }

        public ThemeScripts()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ThemeFiles = ThemeFileResolver.Resolve(CurrentBlog.Theme.Item);
        }
    }
}