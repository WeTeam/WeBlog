using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Layouts;
using Sitecore.Modules.WeBlog.Utilities;

namespace Sitecore.Modules.WeBlog.layouts.WeBlog
{
    public partial class BlogTwitter : BaseSublayout
    {
        protected SublayoutParamHelper Helper { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Helper = new SublayoutParamHelper(this, false);
        }
    }
}