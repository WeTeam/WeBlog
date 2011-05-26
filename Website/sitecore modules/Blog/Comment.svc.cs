using Sitecore.Data;
using Sitecore.Modules.Blog.Managers;

namespace Sitecore.Modules.Blog.Services
{
    public class CommentService : ICommentService
    {
        public ID SubmitComment(ID EntryId, Model.Comment comment)
        {
            return CommentManager.AddCommentToEntry(EntryId, comment);
        }
    }
}
