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
    public class BlogMetaInfo : System.Web.UI.WebControls.WebControl
    {
        Blog currentBlog = BlogManager.GetCurrentBlog();

        protected override void Render(HtmlTextWriter writer)
        {
            // If Live Writer is enabled then add the rsd link
            if (currentBlog.EnableLiveWriter == true)
            {
                string rsdLink = "<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/EviBlog/rsd.ashx?blogid=" + currentBlog.ID + "\"/>" + Environment.NewLine;
                writer.Write(Environment.NewLine);
                writer.Write(rsdLink);
                writer.Write(Environment.NewLine);
            }
            // If RSS is enabled then add rss links
            if (currentBlog.EnableRSS == true)
            {
                string rsslinkUserBlog = "<link rel=\"alternate\" title=\"" + currentBlog.Title + "\"  type=\"application/rss+xml\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/EviBlog/Rss.ashx?blogid=" + currentBlog.ID + "&count=10" + "\" />" + Environment.NewLine;
                string rsslinkAllBlogs = "<link rel=\"alternate\" title=\"10 Latest blog entries\"  type=\"application/rss+xml\" href=\"http://" + WebUtil.GetHostName() + "/sitecore modules/EviBlog/Rss.ashx" + "\" />" + Environment.NewLine;

                writer.Write(rsslinkUserBlog);
                writer.Write(Environment.NewLine);
                writer.Write(rsslinkAllBlogs);
                writer.Write(Environment.NewLine);
            }

        }

    }
}
