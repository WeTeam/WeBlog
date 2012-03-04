using System;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogLayout : System.Web.UI.Page
    {
        protected string GetItemTitle()
        {
            return Sitecore.Context.Item.GetItemTitle();
        }
    }
}