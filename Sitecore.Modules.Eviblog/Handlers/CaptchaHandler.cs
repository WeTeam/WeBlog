using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;

namespace Sitecore.Modules.EviBlog.Handlers
{
    /// <summary>
    /// Summary description for CaptchaHandler
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class CaptchaHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            Bitmap objBMP = new System.Drawing.Bitmap(80, 30);
            Graphics objGraphics = System.Drawing.Graphics.FromImage(objBMP);
            objGraphics.Clear(Color.White);

            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            //' Configure font to use for text
            Font objFont = new Font("Arial", 16, FontStyle.Bold);
            string randomStr = "";
            int[] myIntArray = new int[5];
            int x;

            //That is to create the random # and add it to our string
            Random autoRand = new Random();

            for (x = 0; x < 5; x++)
            {
                myIntArray[x] = System.Convert.ToInt32(autoRand.Next(0, 9));
                randomStr += (myIntArray[x].ToString());
            }

            //This is to add the string to session cookie, to be compared later
            context.Session.Add("CaptchaText", randomStr);

            //' Write out the text
            HatchBrush hb = new HatchBrush(HatchStyle.Sphere, Color.Red, Color.Wheat);
            objGraphics.DrawString(randomStr, objFont, hb, 3, 3);

            //' Set the content type and return the image
            context.Response.ContentType = "image/GIF";
            objBMP.Save(context.Response.OutputStream, ImageFormat.Gif);

            objFont.Dispose();
            objGraphics.Dispose();
            objBMP.Dispose();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
