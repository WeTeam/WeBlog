using System;
using Sitecore.Modules.WeBlog.Items.Blog;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public class BaseEntrySublayout : BaseSublayout
    {
        /// <summary>
        /// Gets or sets the current entry to display from
        /// </summary>
        public EntryItem CurrentEntry
        {
            get;
            set;
        }

        public BaseEntrySublayout()
        {
            CurrentEntry = new EntryItem(Sitecore.Context.Item);
        }
    }
}