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
    public class CommentTitle : System.Web.UI.WebControls.WebControl
    {
        public Item Item { get; set; }
        private Items.Comment currentComment;

        protected override void CreateChildControls()
        {
            if (Item == null)
                currentComment = new Comment(Sitecore.Context.Item);
            else
                currentComment = new Comment(Item);

            //this.Controls.Add(new LiteralControl("<h3>"));
            
            if (currentComment.Website != null && !string.IsNullOrEmpty(currentComment.Website))
            {
                HyperLink link = new HyperLink();
                link.Text = currentComment.UserName;
                link.NavigateUrl = currentComment.Website;
                this.Controls.Add(link);
            }
            else
            {
                this.Controls.Add(new LiteralControl(currentComment.UserName));
            }
            //this.Controls.Add(new LiteralControl("</h3>"));
            
            base.CreateChildControls();
        }
    }
}
