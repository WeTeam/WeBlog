using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface IArchiveCore
    {
        Dictionary<int, List<EntryItem>> EntriesByMonthAndYear { get; set; }
        Dictionary<int, int[]> MonthsByYear { get; set; }
        string GetFriendlyMonthName(int yearAndMonth);
    }
}