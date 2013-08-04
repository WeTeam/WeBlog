using System.Collections.Generic;
using System.Web.UI;

namespace Sitecore.Modules.WeBlog.Globalization
{
    public class TranslatorControl : System.Web.UI.WebControls.WebControl
    {
        public string Key { get; set; }
        public string Arg0 { get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }

        protected override void RenderContents(HtmlTextWriter output)
        {
            List<string> args = null;
            if (Arg0 != null)
            {
                args = new List<string>();
                args.Add(Arg0);
                if (Arg1 != null)
                {
                    args.Add(Arg1);
                }
                if (Arg2 != null)
                {
                    args.Add(Arg2);
                }
            }

            if (args != null && !Sitecore.Context.PageMode.IsPageEditor)
            {
                output.Write(Translator.Format(Key, args.ToArray()));
            }
            else
            {
                output.Write(Translator.Render(Key));
            }
        }
    }

}