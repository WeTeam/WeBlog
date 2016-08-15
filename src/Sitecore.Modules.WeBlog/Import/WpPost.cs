using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Modules.WeBlog.Import.Providers;


namespace Sitecore.Modules.WeBlog.Import
{
    public class WpPost
    {
        #region Properties
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Content { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public List<WpComment> Comments { get; set; }
        #endregion

        #region Constants
        private const string ContentNamespace = "http://purl.org/rss/1.0/modules/content/";
        private const string CommentNamespace = "http://wellformedweb.org/CommentAPI/";
        private const string ElementsNamespace = "http://purl.org/dc/elements/1.1/";
        private const string WordpressNamespace = "http://wordpress.org/export/1.1/";
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WpPost"/> class.
        /// </summary>
        /// <param name="postXml">The post XML.</param>
        public WpPost(XElement postXml)
        {
            GetPost(postXml, false, false, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpPost"/> class.
        /// </summary>
        /// <param name="postXml">The post XML.</param>
        /// <param name="includeComments">if set to <c>true</c> [include comments].</param>
        /// <param name="includeCategories">if set to <c>true</c> [include categories].</param>
        /// <param name="includeTags">if set to <c>true</c> [include tags].</param>
        public WpPost(XElement postXml, bool includeComments, bool includeCategories, bool includeTags)
        {
            GetPost(postXml, includeComments, includeCategories, includeTags);
        }

        public WpPost(XElement postXml, WpImportOptions options)
        {
            GetPost(postXml, options.IncludeComments, options.IncludeCategories, options.IncludeTags);
        }
        #endregion

        #region Private Methods
        private void GetPost(XElement postXml, bool includeComments, bool includeCategories, bool includeTags)
        {
            XNamespace nsContent = ContentNamespace;
            XNamespace wpContent = WordpressNamespace;

            if(postXml.Element("title") != null)
                Title = postXml.Element("title").Value;

            if (postXml.Element(nsContent + "encoded") != null)
                Content = postXml.Element(nsContent + "encoded").Value;

            if (postXml.Element("pubDate") != null)
                PublicationDate = DateTime.Parse(postXml.Element("pubDate").Value);

            Categories = new List<string>();
            Tags = new List<string>();
            Comments = new List<WpComment>();

            if (includeCategories)
            {
                Categories = (from category in postXml.Elements("category")
                              let domain = category.Attribute("domain")
                              where domain != null && domain.Value == "category"
                              orderby category.Value
                              select category.Value).ToList();
            }

            if (includeTags)
            {
                Tags = (from tag in postXml.Elements("category")
                        let domain = tag.Attribute("domain")
                        where domain != null && domain.Value == "post_tag"
                        orderby tag.Value
                        select tag.Value).ToList();
            }

            if (includeComments)
            {
                Comments = (from comment in postXml.Elements(wpContent + "comment")
                            orderby comment.Value
                            select new WpComment(comment)).ToList();
            }
        }
        #endregion
    }
}