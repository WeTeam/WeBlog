using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface IEntryNavigationCore
    {
        EntryItem GetPreviousEntry(EntryItem entry);
        EntryItem GetNextEntry(EntryItem entry);
    }
}