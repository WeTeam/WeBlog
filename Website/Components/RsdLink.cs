using System.Collections.Generic;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Web;

namespace Sitecore.Modules.WeBlog.Components
{
    public class RsdLink : IRsdInclude
    {
        protected BlogHomeItem Blog;
        public Dictionary<HtmlTextWriterAttribute, string> Attributes { get; set; }

        public virtual bool ShouldInclude
        {
            get
            {
                // If Live Writer is enabled then add the rsd link
                return Blog != null && Blog.EnableLiveWriter.Checked;
            }
        }

        public RsdLink()
        {
            Blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            Attributes = new Dictionary<HtmlTextWriterAttribute, string>
            {
                {HtmlTextWriterAttribute.Href, "http://" + WebUtil.GetHostName() + "/sitecore modules/WeBlog/rsd.ashx?blogid=" + Blog.ID},
                {HtmlTextWriterAttribute.Rel, "EditURI"},
                {HtmlTextWriterAttribute.Type, "application/rsd+xml"},
                {HtmlTextWriterAttribute.Title, "RSD"}
            };
        }
    }
}