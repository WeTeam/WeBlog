using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MSCaptcha;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class Captcha : BlogRenderingModelBase
    {
        protected CaptchaImage CaptchaImage { get; set; }
        public string ChallengeValue { get; set; }
        public string ImageSource { get; set; }

        public Captcha()
        {
            CaptchaImage = new CaptchaImage();
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            ChallengeValue = Crypto.GetMD5Hash(CaptchaImage.Text);
            ImageSource = BuildImageSource();

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