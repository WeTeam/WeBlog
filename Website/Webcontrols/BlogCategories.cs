using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Blog.Items.Blog;
using Sitecore.Modules.Blog.Managers;

namespace Sitecore.Modules.Blog.WebControls
{
    public class BlogCategories : System.Web.UI.WebControls.WebControl
    {
        protected override void CreateChildControls()
        {
            var listCategories = CategoryManager.GetCategories();

            if (listCategories.Length != 0)
            {
                Controls.Add(new LiteralControl("<ul>"));

                foreach (CategoryItem item in listCategories)
                {
                    Web.UI.WebControls.Text txt = new Web.UI.WebControls.Text();
                    txt.Field = "Title";
                    txt.DataSource = item.ID.ToString();

                    HyperLink postLink = new HyperLink();
                    postLink.NavigateUrl = LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(item.ID));
                    postLink.Controls.Add(txt);

                    Controls.Add(new LiteralControl("<li>"));
                    Controls.Add(postLink);
                    Controls.Add(new LiteralControl("</li>"));
                }
                Controls.Add(new LiteralControl("</ul>"));
            }
        }

    }
}
