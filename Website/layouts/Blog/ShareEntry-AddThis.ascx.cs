using System;

namespace Sitecore.Modules.WeBlog.layouts.Blog
{
    public partial class ShareEntry_AddThis : System.Web.UI.UserControl
    {
        protected string AddThisAccountName
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var addThisAccountName = Sitecore.Configuration.Settings.GetSetting("Blog.AddThisAccountName");
            AddThisAccountName = string.IsNullOrEmpty(addThisAccountName) ? "" : "#pubid=" + addThisAccountName;
            DataBind();
        }
    }
}