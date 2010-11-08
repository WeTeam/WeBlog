using System;
using Sitecore.Data.Items;
using System.Text;
using Microsoft.Security.Application;

namespace Sitecore.Modules.Eviblog.Items
{
    public class Comment : CustomItemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        /// <param name="item">The comment.</param>
        public Comment(Item item)
            : base(item)
        {
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        /// <value>The name.</value>
        public string UserName
        {
            get
            {
                return InnerItem["Name"];
            }
            set
            {
                InnerItem["Name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email
        {
            get
            {
                return InnerItem["Email"];
            }
            set
            {
                InnerItem["Email"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the website.
        /// </summary>
        /// <value>The website.</value>
        public string Website
        {
            get
            {
                return InnerItem["Website"];
            }
            set
            {
                InnerItem["Website"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the author's IP address
        /// </summary>
        public string IPAddress
        {
            get
            {
                return InnerItem["IP Address"];
            }
            set
            {
                InnerItem["IP Address"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string CommentText
        {
            get
            {
                return GetSafeCommentText();
            }
            set
            {
                InnerItem["Comment"] = value;
            }
        }

        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                return InnerItem.Statistics.Created;
            }
        }

        /// <summary>
        /// Gets the comment text safe for displaying on the web
        /// </summary>
        private string GetSafeCommentText()
        {
            if (InnerItem != null)
            {
                string text = InnerItem["Comment"];
                string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                StringBuilder output = new StringBuilder();
                foreach (string line in lines)
                {
                    output.Append(AntiXss.HtmlEncode(line));
                    output.Append("<br/>");
                }

                return output.ToString();
            }
            else
                return string.Empty;
        }
    }
}
