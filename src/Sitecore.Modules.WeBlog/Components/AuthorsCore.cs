using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.Components
{
    public class AuthorsCore : IAuthorsCore
    {
        private IEnumerable<string> _users;

        public BlogHomeItem CurrentBlog { get; set; }

        public IEnumerable<string> Users
        {
            get
            {
                if (_users == null)
                {
                    LoadUsers();
                }
                return _users;
            }
            set { _users = value; }
        }

        public AuthorsCore(BlogHomeItem currentBlog)
        {
            CurrentBlog = currentBlog;
        }

        protected virtual void LoadUsers()
        {
            var groupedEntries = ManagerFactory.EntryManagerInstance.GetBlogEntries(CurrentBlog).GroupBy(GetUserFullName).ToList();
            groupedEntries.Sort((g1, g2) => g2.Count() - g1.Count());
            Users = groupedEntries.Select(items => items.Key);
        }

        public virtual string GetUserFullName(EntryItem arg)
        {
            var author = arg.Author;
            if (author.Raw.Contains("\\"))
            {
                if (!User.Exists(author.Raw)) return author.Rendered;

                var user = User.FromName(author.Raw, false);

                if (user == null) return author.Rendered;

                var name = user.LocalName;

                if (!string.IsNullOrEmpty(user.Profile?.FullName))
                {
                    name = user.Profile.FullName;
                }
                return name;
            }
            return author.Raw;
        }

        public virtual string GetAuthorUrl(string author)
        {
            return CurrentBlog.InnerItem.GetUrl() + "?author=" + GetAuthorIdentity(author);
        }

        public virtual string GetAuthorIdentity(string author)
        {
            return Crypto.GetMD5Hash(author);
        }
    }
}