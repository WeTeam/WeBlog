using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Search
{
    public interface IInterstingEntriesAlgorithm
    {
        EntryItem[] GetEntries(Item blog, int maxCount);
    }
}
