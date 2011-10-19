using System;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogLayout : System.Web.UI.Page
    {
        protected string GetItemTitle()
        {
            return Utilities.Presentation.GetItemTitle(Sitecore.Context.Item);
        }
    }
}