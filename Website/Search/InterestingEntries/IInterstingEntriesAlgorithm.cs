using System;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Search
{
    public interface IInterstingEntriesAlgorithm
    {
        EntryItem[] GetEntries(Item blog, int maxCount);
    }
}
