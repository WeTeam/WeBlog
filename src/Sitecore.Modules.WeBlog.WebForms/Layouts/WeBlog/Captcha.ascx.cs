using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Globalization;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.WeBlog
{
    public partial class Captcha : UserControl
    {
        protected void uxCaptchaValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (uxCaptchaCode != null)
            {
                try
                {
                    uxCaptchaCode.ValidateCaptcha(uxCaptchaText.Text);
                    uxCaptchaCode.CaptchaMaxTimeout = (int)Settings.CaptchaMaximumTimeout.TotalSeconds;
                    uxCaptchaCode.CaptchaMinTimeout = (int)Settings.CaptchaMinimumTimeout.TotalSeconds;
                    args.IsValid = uxCaptchaCode.UserValidated;
                }
                catch (NullReferenceException)
                {
                    Page.Response.Redirect(Page.Request.Url.ToString(), true);
                }
            }
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {
            lblCaptcha.Text = Translator.Render("CAPTCHA");
        }

        protected void uxCaptchaCode_RefreshButtonClick(object sender, ImageClickEventArgs arg)
        {
            if (uxCaptchaText != null)
            {
                uxCaptchaText.Focus();
            }
        }
    }
}