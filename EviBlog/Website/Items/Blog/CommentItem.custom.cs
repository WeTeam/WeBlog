using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Modules.Blog.Items.Blog
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