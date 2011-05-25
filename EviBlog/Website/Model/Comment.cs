using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.Modules.Blog.Model
{
    /// <summary>
    /// Represents an abstract comment
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Gets or sets the name of the author
        /// </summary>
        public string AuthorName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the author's email address
        /// </summary>
        public string AuthorEmail
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the author's website URL
        /// </summary>
        public string AuthorWebsite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the IP address the comment was originally submitted from
        /// </summary>
        public string AuthorIP
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text of the comment
        /// </summary>
        public string Text
        {
            get;
            set;
        }
    }
}
