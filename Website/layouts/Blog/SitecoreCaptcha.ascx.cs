using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Modules.Blog.Layouts
{
	public partial class SitecoreCaptcha : System.Web.UI.UserControl
	{
        protected void uxCaptchaValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            uxCaptchaCode.ValidateCaptcha(uxCaptchaText.Text);
            args.IsValid = uxCaptchaCode.UserValidated;
        }

        protected void uxCaptchaCode_RefreshButtonClick(object sender, ImageClickEventArgs arg)
        {
            uxCaptchaText.Focus();
        }
	}
}