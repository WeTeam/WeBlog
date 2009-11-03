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
    public class BlogTitle : System.Web.UI.WebControls.WebControl
    {
        Item currentBlog = BlogManager.GetCurrentBlogItem();
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
