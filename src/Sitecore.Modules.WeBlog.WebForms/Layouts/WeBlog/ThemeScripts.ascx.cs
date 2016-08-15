using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog
{
    public partial class ThemeScripts : BaseSublayout
    {
        protected ThemeItem ThemeItem
        {
            get
            {
                return CurrentBlog.Theme.Item;
            }
        }
    }
}