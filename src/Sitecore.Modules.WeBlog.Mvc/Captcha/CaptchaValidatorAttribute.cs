using System;
using System.Web.Mvc;

namespace Sitecore.Modules.WeBlog.Mvc.Captcha
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CaptchaValidator recaptchaValidator = new CaptchaValidator
            {
                ChallengeValue = filterContext.HttpContext.Request.Form["captcha_challenge_field"],
                ResponseValue = filterContext.HttpContext.Request.Form["captcha_response_field"],
                ClientKey = GetClientKey(filterContext)
            };
            if (recaptchaValidator.ShouldValidate())
            {
                bool valid = recaptchaValidator.Validate();
                if (!valid)
                {
                    InvalidateClientKey(filterContext);
                }
                filterContext.ActionParameters["captchaValid"] = valid;
                base.OnActionExecuting(filterContext);
            }
        }

        private void InvalidateClientKey(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
            {
                filterContext.HttpContext.Session.Remove("Captcha");
            }
        }

        private static string GetClientKey(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
            {
                var captchaText = filterContext.HttpContext.Session["Captcha"];
                if (captchaText != null)
                    return captchaText.ToString();
            }

            return string.Empty;
        }
    }
}