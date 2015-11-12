using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface IPostListCore
    {
        void Initialize(NameValueCollection queryString);
        IEnumerable<EntryItem> Entries { get;  }
        bool ShowViewMoreLink { get;  }
        string ViewMoreHref { get;  }
    }
}