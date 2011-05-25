using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Modules.Blog.layouts.Blog
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