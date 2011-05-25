using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Modules.Blog.Items.Blog;

namespace Sitecore.Modules.Blog.WebControls
{
    public class CommentText : System.Web.UI.WebControls.WebControl
    {
        public Item Item { get; set; }
        private CommentItem currentComment;

        protected override void CreateChildControls()
        {
            if (Item == null)
                currentComment = new CommentItem(Sitecore.Context.Item);
            else
                currentComment = new CommentItem(Item);

            if (!string.IsNullOrEmpty(currentComment.Comment.Text))
            {
                this.Controls.Add(new LiteralControl("<span class=\"comment\">"));
                this.Controls.Add(new LiteralControl(currentComment.Comment.Text));
                this.Controls.Add(new LiteralControl("</span>"));
            }
            
            base.CreateChildControls();
        }
    }
}
