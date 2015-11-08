using System.Text;
using System.Web;
using System.Xml;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Web;

namespace Sitecore.Modules.WeBlog
{
    /// <summary>
    /// RSD (Really Simple Discoverability) Handler
    /// http://cyber.law.harvard.edu/blogs/gems/tech/rsd.html
    /// </summary>
    public class RsdHandler : IHttpHandler
    {

        #region IHttpHandler Members
        /// <summary>
        /// IsReusable implmentation for IHttpHandler
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// Process to return RSD page.
        /// </summary>
        /// <param name="context">context</param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            using (XmlTextWriter rsd = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8))
            {
                Database web = Factory.GetDatabase("web");
                Item currentBlogItem = web.GetItem(new ID(HttpContext.Current.Request.QueryString["blogid"].ToString()));

                BlogHomeItem currentBlog = new BlogHomeItem(currentBlogItem);

                rsd.Formatting = Formatting.Indented;
                rsd.WriteStartDocument();

                // Rsd tag
                rsd.WriteStartElement("rsd");
                rsd.WriteAttributeString("version", "1.0");

                // Service 
                rsd.WriteStartElement("service");
                rsd.WriteElementString("engineName", "Sitecore WeBlog Module");
                rsd.WriteElementString("engineLink", "http://" + WebUtil.GetHostName());
                rsd.WriteElementString("homePageLink", currentBlog.AbsoluteUrl);

                // APIs
                rsd.WriteStartElement("apis");

                // MetaWeblog
                rsd.WriteStartElement("api");
                rsd.WriteAttributeString("name", "MetaWeblog");
                rsd.WriteAttributeString("preferred", "true");
                rsd.WriteAttributeString("apiLink", "http://" + WebUtil.GetHostName() + "/sitecore modules/WeBlog/MetaBlogApi.ashx");
                rsd.WriteAttributeString("blogID", currentBlog.ID.ToString());
                rsd.WriteEndElement();

                //// BlogML
                //rsd.WriteStartElement("api");
                //rsd.WriteAttributeString("name", "BlogML");
                //rsd.WriteAttributeString("preferred", "false");
                //rsd.WriteAttributeString("apiLink", Utils.AbsoluteWebRoot + "api/BlogImporter.asmx");
                //rsd.WriteAttributeString("blogID", Utils.AbsoluteWebRoot.ToString());
                //rsd.WriteEndElement();

                // End APIs
                rsd.WriteEndElement();

                // End Service
                rsd.WriteEndElement();

                // End Rsd
                rsd.WriteEndElement();

                rsd.WriteEndDocument();

            }
        }
        #endregion
    }
}
