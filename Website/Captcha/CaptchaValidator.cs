using System;

namespace Sitecore.Modules.WeBlog.Captcha
{
    public class CaptchaValidator
    {
        public string ChallengeValue { get; set; }
        public string ResponseValue { get; set; }

        public bool Validate()
        {
            if (!String.IsNullOrEmpty(ResponseValue))
            {
                var responseHash = Crypto.GetMD5Hash(ResponseValue);
                return responseHash.Equals(ChallengeValue);
            }
            return false;
        }

        public bool ShouldValidate()
        {
            return ChallengeValue != null && ResponseValue != null;
        }
    }
}