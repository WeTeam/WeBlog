using System;
using Sitecore.Modules.WeBlog.Configuration;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    public partial class ReCaptcha : System.Web.UI.UserControl
    {
        protected IWeBlogSettings Settings { get; }

        public ReCaptcha()
            : this(WeBlogSettings.Instance)
        {
        }

        public ReCaptcha(IWeBlogSettings settings)
        {
            Settings = settings;
        }

        protected void uxRecaptcha_Init(object sender, EventArgs e)
        {
            Recaptcha.RecaptchaControl recaptcha = sender as Recaptcha.RecaptchaControl;
            if (recaptcha != null)
            {
                recaptcha.PublicKey = Settings.ReCaptchaPublicKey;
                recaptcha.PrivateKey = Settings.ReCaptchaPrivateKey;
            }
            lblCaptcha.Text = Globalization.Translator.Render("CAPTCHA");
        }
    }
}