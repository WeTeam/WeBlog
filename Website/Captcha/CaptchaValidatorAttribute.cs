using System;
using System.Web.Mvc;

namespace Sitecore.Modules.WeBlog.Captcha
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
                filterContext.ActionParameters["captchaValid"] = recaptchaValidator.Validate();
                base.OnActionExecuting(filterContext);
            }
        }

        private static string GetClientKey(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
            {
                return filterContext.HttpContext.Session["Captcha"].ToString();
            }
            return String.Empty;
        }
    }
}