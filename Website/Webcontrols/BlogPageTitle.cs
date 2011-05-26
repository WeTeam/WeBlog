using System.Web.UI;
using Sitecore.Modules.Blog.Managers;

namespace Sitecore.Modules.Blog.WebControls
{
    public class BlogPageTitle : System.Web.UI.WebControls.WebControl
    {
        Sitecore.Modules.Blog.Items.Blog.BlogItem currentBlog = BlogManager.GetCurrentBlog();

        protected override void Render(HtmlTextWriter writer)
        {
            string pageTitle = string.Empty;

            if (Sitecore.Context.Item.TemplateID.ToString() == Sitecore.Configuration.Settings.GetSetting("Blog.EntryTemplateID"))
            {
                Items.Blog.EntryItem currentEntry = EntryManager.GetCurrentBlogEntry();

                pageTitle = currentBlog.Title.Text + " - " + currentEntry.Title;
            }
            else
            {
                pageTitle = currentBlog.Title.Text;
            }
            writer.Write(pageTitle);
        }

    }
}
