using System;
using Sitecore.Modules.WeBlog.Configuration;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    public partial class ShareEntryShareThis : System.Web.UI.UserControl
    {
        protected IWeBlogSettings Settings { get; }

        protected string ShareThisPublisherID
        {
            get;
            set;
        }

        public ShareEntryShareThis()
            : this(WeBlogSettings.Instance)
        {
        }

        public ShareEntryShareThis(IWeBlogSettings settings)
        {
            Settings = settings;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ShareThisPublisherID = Settings.ShareThisPublisherId;
            DataBind();
        }
    }
}