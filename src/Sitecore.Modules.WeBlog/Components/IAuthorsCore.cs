using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface IAuthorsCore
    {
        IEnumerable<string> Users { get; set; }
        string GetAuthorUrl(string author);
        string GetAuthorIdentity(string author);
        string GetUserFullName(EntryItem arg);
    }
}