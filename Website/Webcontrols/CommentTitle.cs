using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Modules.Blog.Items.Blog;

namespace Sitecore.Modules.Blog.WebControls
{
    public class CommentTitle : System.Web.UI.WebControls.WebControl
    {
        public Item Item { get; set; }
        private Items.Blog.CommentItem currentComment;

        protected override void CreateChildControls()
        {
            if (Item == null)
                currentComment = new CommentItem(Sitecore.Context.Item);
            else
                currentComment = new CommentItem(Item);

            //this.Controls.Add(new LiteralControl("<h3>"));
            
            if (currentComment.Website != null && !string.IsNullOrEmpty(currentComment.Website.Text))
            {
                HyperLink link = new HyperLink();
                link.Text = currentComment.Name.Text;
                link.NavigateUrl = currentComment.Website.Text;
                this.Controls.Add(link);
            }
            else
            {
                this.Controls.Add(new LiteralControl(currentComment.Name.Text));
            }
            //this.Controls.Add(new LiteralControl("</h3>"));
            
            base.CreateChildControls();
        }
    }
}
