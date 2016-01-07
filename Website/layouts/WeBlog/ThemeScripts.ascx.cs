using System;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class ThemeScripts : BaseSublayout
    {
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
    }
}