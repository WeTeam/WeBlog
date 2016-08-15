using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog
{
    public partial class BlogLayout : System.Web.UI.Page
    {
        protected string GetItemTitle()
        {
            return Sitecore.Context.Item.GetItemTitle();
        }
    }
}