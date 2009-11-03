using System.Collections.Generic;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Data.Items;

namespace Sitecore.Modules.Eviblog.Comparers
{
    public class PostDateComparerDesc : IComparer<Entry>
    {
        /// <summary>
        /// Compares the specified first.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public int Compare(Entry first, Entry second)
        {
            if (first.Created > second.Created)
            {
                return -1;
            }
            if (first.Created < second.Created)
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

    public class CommentDateComparerDesc : IComparer<Comment>
    {
        /// <summary>
        /// Compares the specified first.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <returns></returns>
        public int Compare(Comment first, Comment second)
        {
            if (first.Created > second.Created)
            {
                return 1;
            }
            if (first.Created < second.Created)
            {
                return -1;
            }
            return 0;
        }
    }
}
