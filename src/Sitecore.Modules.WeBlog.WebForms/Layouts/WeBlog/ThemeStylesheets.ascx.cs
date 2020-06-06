using System;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Themes;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    [AllowDependencyInjection]
    public partial class ThemeStylesheets : BaseSublayout
    {
        protected IThemeFileResolver ThemeFileResolver { get; }

        [Obsolete("No longer supported.")]
        public ThemeItem ThemeItem
        {
            get
            {
                var rawThemeItem = CurrentBlog.Theme.Item;
                if (rawThemeItem == null)
                    return null;

                var theme = (ThemeItem)rawThemeItem;
                if (theme == null)
                    return null;

                return theme;
            }
        }

        public ThemeStylesheets(IThemeFileResolver themeFileResolver)
        {
            ThemeFileResolver = themeFileResolver;
        }

        public ThemeStylesheets()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var themeFiles = ThemeFileResolver.Resolve(CurrentBlog.Theme.Item);

            if (themeFiles.Stylesheets != null)
            {
                
                Stylesheets.DataSource = themeFiles.Stylesheets;
                Stylesheets.DataBind();
            }
        }
    }
}