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
                ResponseValue = filterContext.HttpContext.Request.Form["captcha_response_field"]
            };
            if (recaptchaValidator.ShouldValidate())
            {
                filterContext.ActionParameters["captchaValid"] = recaptchaValidator.Validate();
                base.OnActionExecuting(filterContext);
            }
        }
    }
}