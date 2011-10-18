using System;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public partial class CommentItem
    {
        /// <summary>
        /// Gets the creation date of the comment item.
        /// </summary>
        public DateTime Created
        {
            get
            {
                return InnerItem.Statistics.Created;
            }
        }

        /// <summary>
        /// For use with NVelocity token in email template
        /// </summary>
        public string AuthorName
        {
            get
            {
                return this.Name.Raw;
            }
        }
    }
}