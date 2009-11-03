using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml.Xsl;
using Sitecore.Web;
using Sitecore.Collections;
using Sitecore.Data.Items;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.Eviblog.Items;

namespace Sitecore.Modules.Eviblog.Webcontrols
{
    public class CommentText : System.Web.UI.WebControls.WebControl
    {
        public Item Item { get; set; }
        private Comment currentComment;

        protected override void CreateChildControls()
        {
            if (Item == null)
                currentComment = new Comment(Sitecore.Context.Item);
            else
                currentComment = new Comment(Item);

            if (!string.IsNullOrEmpty(currentComment.CommentText))
            {
                this.Controls.Add(new LiteralControl("<span class=\"comment\">"));
                this.Controls.Add(new LiteralControl(currentComment.CommentText));
                this.Controls.Add(new LiteralControl("</span>"));
            }
            
            base.CreateChildControls();
        }
    }
}
