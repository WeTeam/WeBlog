using System;
using Sitecore.Data.Items;

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
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string CommentText
        {
            get
            {
                return InnerItem["Comment"];
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
    }
}
