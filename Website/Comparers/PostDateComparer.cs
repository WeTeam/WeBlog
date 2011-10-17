using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Comparers
{
    public class PostDateComparerDesc : IComparer<EntryItem>
    {
        /// <summary>
        /// Compares the specified first.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public int Compare(EntryItem first, EntryItem second)
        {
            if (first.InnerItem.Statistics.Created > second.InnerItem.Statistics.Created)
            {
                return -1;
            }
            if (first.InnerItem.Statistics.Created < second.InnerItem.Statistics.Created)
            {
                return 1;
            }
            return 0;
        }
    }

    public class ItemDateComparerDesc : IComparer<Item>
    {
        /// <summary>
        /// Compares the specified first.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public int Compare(Item first, Item second)
        {
            if (first.Statistics.Created > second.Statistics.Created)
            {
                return -1;
            }
            if (first.Statistics.Created < second.Statistics.Created)
            {
                return 1;
            }
            return 0;
        }
    }

    public class CommentDateComparerDesc : IComparer<CommentItem>
    {
        /// <summary>
        /// Compares the specified first.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public int Compare(CommentItem first, CommentItem second)
        {
            if (first.InnerItem.Statistics.Created > second.InnerItem.Statistics.Created)
            {
                return 1;
            }
            if (first.InnerItem.Statistics.Created < second.InnerItem.Statistics.Created)
            {
                return -1;
            }
            return 0;
        }
    }
}
