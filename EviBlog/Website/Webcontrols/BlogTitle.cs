using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.Blog.WebControls
{
    public class BlogTitle : System.Web.UI.WebControls.WebControl
    {
        Item currentBlog = BlogManager.GetCurrentBlog().InnerItem;
        public bool UseLink { get; set; }

        protected override void CreateChildControls()
        {
            if (!UseLink)
            {
                FieldRenderer renderer = new FieldRenderer { Item = currentBlog, FieldName = "Title" };
                Controls.Add(renderer);
            }
            else
            {
                FieldRenderer renderer = new FieldRenderer { Item = currentBlog, FieldName = "Title" };
                HyperLink link = new HyperLink { NavigateUrl = LinkManager.GetItemUrl(currentBlog) };
                
                link.Controls.Add(renderer);
                Controls.Add(link);
            }

            base.CreateChildControls();
        }

    }
}
