using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Recaptcha;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Mvc.Helpers;

namespace Sitecore.Modules.WeBlog.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString Field(this SitecoreHelper htmlHelper, string fieldName, bool shouldWrap)
        {
            return Field(htmlHelper, fieldName, null, null, shouldWrap);
        }

        public static HtmlString Field(this SitecoreHelper htmlHelper, string fieldName, object parameters, bool shouldWrap)
        {
            return Field(htmlHelper, fieldName, null, parameters, shouldWrap);
        }

        public static HtmlString Field(this SitecoreHelper htmlHelper, string fieldName, Item item, bool shouldWrap)
        {
            return Field(htmlHelper, fieldName, item, null, shouldWrap);
        }

        public static HtmlString Field(this SitecoreHelper htmlHelper, string fieldName, Item item, object parameters, bool shouldWrap)
        {
            string format = shouldWrap ? "<p>{0}{1}</p>" : "{0}{1}";
            var value = String.Format(format,
                htmlHelper.BeginField(fieldName, item, parameters),
                htmlHelper.EndField());
            return new HtmlString(value);
        }

        [Obsolete("Captcha is deprecated. Use IValidateCommentCore.Validate() from the service provider instead.")]
        public static string GenerateReCaptcha(this HtmlHelper helper, string id, string theme)
        {
            if (string.IsNullOrEmpty(WeBlogSettings.Instance.ReCaptchaPublicKey) || string.IsNullOrEmpty(WeBlogSettings.Instance.ReCaptchaPrivateKey))
                throw new ApplicationException("reCAPTCHA needs to be configured with a public & private key.");
            RecaptchaControl recaptchaControl1 = new RecaptchaControl();
            recaptchaControl1.ID = id;
            recaptchaControl1.Theme = theme;
            recaptchaControl1.PublicKey = WeBlogSettings.Instance.ReCaptchaPublicKey;
            recaptchaControl1.PrivateKey = WeBlogSettings.Instance.ReCaptchaPrivateKey;
            RecaptchaControl recaptchaControl2 = recaptchaControl1;
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());
            recaptchaControl2.RenderControl(writer);
            return writer.InnerWriter.ToString();
        }
    }
}