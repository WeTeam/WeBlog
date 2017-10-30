using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class Authors : BlogRenderingModelBase
    {
        public IEnumerable<string> AuthorsList { get; set; }

        public IAuthorsCore AuthorsCore { get; set; }

        public int MaximumCount { get; set; }

        public Authors() : this(null) { }

        public Authors(IAuthorsCore authorsCore)
        {
            AuthorsCore = authorsCore ?? new AuthorsCore(CurrentBlog);
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            AuthorsList = AuthorsCore.Users.Take(MaximumCount);
        }
    }
}