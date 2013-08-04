using System;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class ShareEntryShareThis : System.Web.UI.UserControl
    {
        protected string ShareThisPublisherID
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ShareThisPublisherID = Settings.ShareThisPublisherID;
            DataBind();
        }
    }
}