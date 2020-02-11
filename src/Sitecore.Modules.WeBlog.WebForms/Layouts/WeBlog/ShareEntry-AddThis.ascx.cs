using System;
using Sitecore.Modules.WeBlog.Configuration;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    public partial class ShareEntryAddThis : System.Web.UI.UserControl
    {
        protected IWeBlogSettings Settings { get; }

        public ShareEntryAddThis()
            : this(WeBlogSettings.Instance)
        {
        }

        public ShareEntryAddThis(IWeBlogSettings settings)
        {
            Settings = settings;
        }

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