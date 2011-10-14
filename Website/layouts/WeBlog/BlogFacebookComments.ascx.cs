using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Utilities;

namespace Sitecore.Modules.WeBlog.layouts.WeBlog
{
    public partial class BlogFacebookComments : System.Web.UI.UserControl
    {
        protected SublayoutParamHelper Helper { get; set; }
        protected string UrlToCommentOn;

        protected void Page_Load(object sender, EventArgs e)
        {
            Helper = new SublayoutParamHelper(this, false);

            UrlToCommentOn = Helper.GetParam("Url to Comment on");
            if (string.IsNullOrEmpty(UrlToCommentOn))
            {
                // Use current page
                UrlToCommentOn = Sitecore.Links.LinkManager.GetItemUrl(Sitecore.Context.Item, new Sitecore.Links.UrlOptions { AlwaysIncludeServerUrl = true });
            }
        }
    }
}