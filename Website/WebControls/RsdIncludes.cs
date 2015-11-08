using System.Web.UI;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Web;
using Sitecore.Web.UI;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class RsdIncludes : WebControl
    {
        protected override void DoRender(HtmlTextWriter output)
        {
            var blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();

            // If Live Writer is enabled then add the rsd link
            if (blog != null && blog.EnableLiveWriter.Checked)
            {
                AddLinkToOutput(output, blog);
            }
        }

        protected virtual void AddLinkToOutput(HtmlTextWriter output, BlogHomeItem blogItem)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Rel, "EditURI");
            output.AddAttribute(HtmlTextWriterAttribute.Type, "application/rsd+xml");
            output.AddAttribute(HtmlTextWriterAttribute.Title, "RSD");
            output.AddAttribute(HtmlTextWriterAttribute.Href, "http://" + WebUtil.GetHostName() + "/sitecore modules/WeBlog/rsd.ashx?blogid=" + blogItem.ID);
            output.RenderBeginTag(HtmlTextWriterTag.Link);
            output.RenderEndTag();
        }
    }
}