using System;

namespace Sitecore.Modules.WeBlog.Items.Blog
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
    }
}