using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.UnitTest.Comparers
{
    class ItemIdentityComparer : IComparer<Item>
    {
        public int Compare(Item x, Item y)
        {
            if (x.Uri == y.Uri)
                return 0;

            return 1;
        }
    }
}
