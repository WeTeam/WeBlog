using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml.Xsl;
using Sitecore.Web;
using Sitecore.Collections;

namespace Sitecore.Modules.Eviblog.Webcontrols
{
    public class EntryIntroduction : Sitecore.Web.UI.WebControls.FieldControl
    {
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            this.Field = "Introduction";
            
            base.DoRender(output);
        }
    }
}
