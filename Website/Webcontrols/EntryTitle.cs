using System;

namespace Sitecore.Modules.Blog.WebControls
{
    public class EntryTitle : Sitecore.Web.UI.WebControls.FieldControl
    {
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            this.Field = "Title";
            
            base.DoRender(output);
        }
    }
}
