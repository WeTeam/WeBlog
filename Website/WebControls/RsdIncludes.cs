using System.Web.UI;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Web;
using Sitecore.Web.UI;

namespace Sitecore.Modules.WeBlog.WebControls
{
    public class RsdIncludes : WebControl
    {
        protected override void DoRender(HtmlTextWriter output)
        {
            var blog = BlogManager.GetCurrentBlog();

            // If Live Writer is enabled then add the rsd link
            if (blog.EnableLiveWriter.Checked)
            {
                AddLinkToOutput(output, blog);
            }
        }

        protected virtual void AddLinkToOutput(HtmlTextWriter output, BlogHomeItem blogItem)
        {
            output.AddAttribute(HtmlTextWriterAttribute.Rel, "application/rsd+xml");
            output.AddAttribute(HtmlTextWriterAttribute.Title, "RSD");
            output.AddAttribute(HtmlTextWriterAttribute.Href, "http://" + WebUtil.GetHostName() + "/sitecore modules/Blog/rsd.ashx?blogid=" + blogItem.ID);
            output.RenderBeginTag(HtmlTextWriterTag.Link);
            output.RenderEndTag();
        }
    }
}