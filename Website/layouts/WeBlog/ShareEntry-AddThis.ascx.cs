using System;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class ShareEntryAddThis : System.Web.UI.UserControl
    {
        protected string AddThisAccountName
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var addThisAccountName = Settings.AddThisAccountName;
            AddThisAccountName = string.IsNullOrEmpty(addThisAccountName) ? "" : "#pubid=" + addThisAccountName;
            DataBind();
        }
    }
}