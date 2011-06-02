using System;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class reCaptcha : System.Web.UI.UserControl
    {
        const string SETTING_PRIVATE_KEY = "WeBlog.reCAPTCHA.PrivateKey";
        const string SETTING_PUBLIC_KEY = "WeBlog.reCAPTCHA.PublicKey";

        protected void uxRecaptcha_Init(object sender, EventArgs e)
        {
            Recaptcha.RecaptchaControl recaptcha = sender as Recaptcha.RecaptchaControl;
            recaptcha.PublicKey = Sitecore.Configuration.Settings.GetSetting(SETTING_PUBLIC_KEY);
            recaptcha.PrivateKey = Sitecore.Configuration.Settings.GetSetting(SETTING_PRIVATE_KEY);
        }
    }
}