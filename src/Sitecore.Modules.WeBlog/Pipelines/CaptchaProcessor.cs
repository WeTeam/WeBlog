using System;
using System.Web;
using Sitecore.Pipelines.PreprocessRequest;

namespace Sitecore.Modules.WeBlog.Pipelines
{
  [Obsolete]
    public class CaptchaProcessor : PreprocessRequestProcessor
  {
    public override void Process(PreprocessRequestArgs args)
    {
      if (HttpContext.Current.Request.RawUrl.Contains("CaptchaImage.axd"))
      {
        var handler = (new MSCaptcha.captchaImageHandler()) as IHttpHandler;
        handler.ProcessRequest(HttpContext.Current);
      }
    }
  }
}