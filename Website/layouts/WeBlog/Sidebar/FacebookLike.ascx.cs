using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Modules.WeBlog.Layouts.WeBlog
{
    public partial class BlogFacebookLike : System.Web.UI.UserControl
    {
        protected SublayoutParamHelper Helper { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Helper = new SublayoutParamHelper(this, false);
        }
    }
}