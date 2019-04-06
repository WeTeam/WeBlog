using System.Web;
using Sitecore.Pipelines.PreprocessRequest;

namespace Sitecore.Modules.WeBlog.Pipelines
{
  public class CaptchaProcessor : PreprocessRequestProcessor
  {
    public override void Process(PreprocessRequestArgs args)
    {
      if (HttpContext.Current.Request.RawUrl.Contains("CaptchaImage.axd"))
      {
        var handler = (new MSCaptcha.CaptchaImageHandler()) as IHttpHandler;
        handler.ProcessRequest(args.Context);
      }
    }
	}
}