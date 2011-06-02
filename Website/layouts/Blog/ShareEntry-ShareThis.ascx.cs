using System;

namespace Sitecore.Modules.WeBlog.layouts.Blog
{
    public partial class ShareEntry_ShareThis : System.Web.UI.UserControl
    {
        protected string ShareThisPublisherID
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ShareThisPublisherID = Sitecore.Configuration.Settings.GetSetting("Blog.ShareThisPublisherID");
            DataBind();
        }
    }
}