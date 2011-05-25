using System;

namespace Sitecore.Modules.Blog.WebControls
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
