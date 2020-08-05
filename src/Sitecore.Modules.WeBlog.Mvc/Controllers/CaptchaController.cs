using MSCaptcha;
using Sitecore.Modules.WeBlog.Mvc.Model;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;

namespace Sitecore.Modules.WeBlog.Mvc.Controllers
{
    [Obsolete("Captcha is deprecated. Use IValidateCommentCore.Validate() from the service provider instead.")]
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
                ChallengeValue = BuildChallangeValue(),
                ImageSource = BuildImageSource()
            };
            return View("~/Views/WeBlog/Captcha.cshtml", model);
        }

        protected virtual string BuildChallangeValue()
        {
            var clientKey = GenerateClientKey();
            StoreClientKey(clientKey);
            return Crypto.GetMD5Hash(string.Format("{0}-{1}", clientKey, CaptchaImage.Text));
        }

        protected virtual void StoreClientKey(string clientKey)
        {
            if (Session["Captcha"] == null)
            {
                Session["Captcha"] = clientKey;
            }
        }

        protected virtual string GenerateClientKey()
        {
            return DateTime.Now.ToShortDateString() + CaptchaImage.UniqueId;
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