using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using MSCaptcha;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class CaptchaController : Controller
    {
        protected CaptchaImage CaptchaImage { get; set; }

        public CaptchaController()
        {
            CaptchaImage = new CaptchaImage();
        }

        public ActionResult Index()
        {            
            CaptchaRenderingModel model = new CaptchaRenderingModel
            {
                ChallengeValue = Crypto.GetMD5Hash(CaptchaImage.Text),
                ImageSource = BuildImageSource()
        };
            return View("~/Views/WeBlog/Captcha.cshtml", model);
        }

        protected virtual string BuildImageSource()
        {
            var bytes = GetImageBytes();
            var base64 = System.Convert.ToBase64String(bytes);
            var src = String.Format("data:image/png;base64,{0}", base64);
            return src;
        }

        protected virtual byte[] GetImageBytes()
        {
            Bitmap bitmap = CaptchaImage.RenderImage();
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Bmp);
            return stream.ToArray();
        }
    }
}