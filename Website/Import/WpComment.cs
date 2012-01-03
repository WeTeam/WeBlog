using System;
using System.Xml.Linq;

namespace Sitecore.Modules.WeBlog.Import
{
    public class WpComment
    {
        #region Properties
        public string Author { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string IP { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public bool Pingback { get; set; }
        #endregion

        #region Constants
        private const string WordpressNamespace = "http://wordpress.org/export/1.1/";
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WpComment"/> class.
        /// </summary>
        /// <param name="commentXml">The comment XML.</param>
        public WpComment(XElement commentXml)
        {
            XNamespace wpContent = WordpressNamespace;

            Author = commentXml.Element(wpContent + "comment_author").Value;
            Email = commentXml.Element(wpContent + "comment_author_email").Value;
            Url = commentXml.Element(wpContent + "comment_author_url").Value;
            IP = commentXml.Element(wpContent + "comment_author_IP").Value;
            Date = DateTime.Parse(commentXml.Element(wpContent + "comment_date").Value);
            Content = commentXml.Element(wpContent + "comment_content").Value;
            Pingback = commentXml.Element(wpContent + "comment_type").Value == "pingback";
        }
        #endregion

        public static implicit operator Model.Comment(WpComment wpComment)
        {
            var model = new Model.Comment();
            model.Text = wpComment.Content;
            model.AuthorEmail = wpComment.Email;
            model.AuthorName = wpComment.Author;
            model.Fields.Add(Constants.Fields.IpAddress, wpComment.IP);
            model.Fields.Add(Constants.Fields.Website, wpComment.Url);
            model.Fields.Add(Sitecore.FieldIDs.Created.ToString(), Sitecore.DateUtil.ToIsoDate(wpComment.Date));

            return model;
        }
    }
}