using System;
using System.Web;
using Sitecore.Pipelines.PreprocessRequest;

namespace Sitecore.Modules.WeBlog.Pipelines
{
  public class CaptchaProcessor : PreprocessRequestProcessor
  {
    [Obsolete]
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