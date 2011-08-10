using System;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class reCaptcha : System.Web.UI.UserControl
    {
        protected void uxRecaptcha_Init(object sender, EventArgs e)
        {
            Recaptcha.RecaptchaControl recaptcha = sender as Recaptcha.RecaptchaControl;
            if (recaptcha != null)
            {
                recaptcha.PublicKey = Settings.ReCaptchaPublicKey;
                recaptcha.PrivateKey = Settings.ReCaptchaPrivateKey;
            }
        }
    }
}