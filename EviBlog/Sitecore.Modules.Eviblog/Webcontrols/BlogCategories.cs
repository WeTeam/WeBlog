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
    public class BlogCategories : System.Web.UI.WebControls.WebControl
    {
        protected override void CreateChildControls()
        {
            List<Category> listCategories = CategoryManager.GetCategories();

            if (listCategories.Count != 0)
            {
                Controls.Add(new LiteralControl("<ul>"));

                foreach (Category item in listCategories)
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
