using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface IAuthorsCore
    {
        IEnumerable<string> Users { get; set; }
        string GetAuthorUrl(string author);
        string GetAuthorIdentity(string author);
        string GetUserFullName(string author);
    }
}