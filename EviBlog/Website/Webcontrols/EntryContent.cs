using System;

namespace Sitecore.Modules.Blog.WebControls
{
    public class EntryContent : Sitecore.Web.UI.WebControls.FieldControl
    {
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            this.Field = "Content";
            
            base.DoRender(output);
        }
    }
}
