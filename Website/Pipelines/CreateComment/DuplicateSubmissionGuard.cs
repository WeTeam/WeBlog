using Sitecore.Diagnostics;
using Sitecore.StringExtensions;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class DuplicateSubmissionGuard : ICreateCommentProcessor
    {
        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.Database, "Database cannot be null");
            Assert.IsNotNull(args.Comment, "Comment cannot be null");
            Assert.IsNotNull(args.EntryID, "Entry ID cannot be null");

            var entryItem = args.Database.GetItem(args.EntryID, args.Language);
            if (entryItem != null)
            {
                var query = "fast:{0}//*[@email='{1}' and @name='{2}' and @comment='{3}']".FormatWith(
                    entryItem.Paths.FullPath, args.Comment.AuthorEmail, args.Comment.AuthorName, args.Comment.Text);
                var comments = args.Database.SelectItems(query);

                if(comments.Length > 0)
                    args.AbortPipeline();
            }
        }
    }
}