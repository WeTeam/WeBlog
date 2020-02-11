using System.Collections.Generic;
using System.Linq;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Components
{
    public class RecentCommentsCore : IRecentCommentsCore
    {
        public IList<EntryComment> Comments { get; protected set; }

        protected BlogHomeItem CurrentBlog { get; set; }

        protected ICommentManager CommentManager { get; }

        protected IEntryManager EntryManager { get; }

        public RecentCommentsCore(IBlogManager blogManager = null, ICommentManager commentManager = null, IEntryManager entryManager = null)
        {
            CurrentBlog = (blogManager ?? ManagerFactory.BlogManagerInstance).GetCurrentBlog();
            CommentManager = commentManager ?? ManagerFactory.CommentManagerInstance;
            EntryManager = entryManager ?? ManagerFactory.EntryManagerInstance;
        }

        public void Initialise()
        {
            var totalToShow = CurrentBlog.DisplayCommentSidebarCountNumeric;

            var blogComments = CommentManager.GetBlogComments(CurrentBlog, totalToShow);
            var commentContents = blogComments.Select(x => CommentManager.GetCommentContent(x));

            Comments = (from comment in commentContents
                let entryItem = EntryManager.GetBlogEntryItemByCommentUri(comment.Uri)
                where entryItem != null
                select new EntryComment
                {
                    Comment = comment,
                    Entry = entryItem
                }).ToList();
        }
    }
}