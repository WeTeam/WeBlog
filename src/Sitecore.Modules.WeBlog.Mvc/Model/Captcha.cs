using System;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    [Obsolete("Captcha is deprecated. Use IValidateCommentCore.Validate() from the service provider instead.")]
    public class CaptchaRenderingModel
    {
        public string ChallengeValue { get; set; }
        public string ImageSource { get; set; }
    }
}