using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.WebControls;
using Sitecore.Xml.Xsl;
using Sitecore.Web;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Data.Items;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;

namespace Sitecore.Modules.Eviblog.Webcontrols
{
    public class BlogPageTitle : System.Web.UI.WebControls.WebControl
    {
        Blog currentBlog = BlogManager.GetCurrentBlog();

        protected override void Render(HtmlTextWriter writer)
        {
            string pageTitle = string.Empty;

            if (Sitecore.Context.Item.TemplateID.ToString() == Settings.Default.EntryTemplateID)
            {
                Items.Entry currentEntry = EntryManager.GetCurrentBlogEntry();

                pageTitle = currentBlog.Title + " - " + currentEntry.Title;
            }
            else
            {
                pageTitle = currentBlog.Title;
            }
            writer.Write(pageTitle);
        }

    }
}
