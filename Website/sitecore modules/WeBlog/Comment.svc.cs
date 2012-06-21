using Sitecore.Data;
using Sitecore.Modules.WeBlog.Managers;
using System.Runtime.Serialization;

namespace Sitecore.Modules.WeBlog.Services
{
    public class CommentService : ICommentService
    {
        public ID SubmitComment(ID EntryId, Model.Comment comment)
        {
            return ManagerFactory.CommentManagerInstance.AddCommentToEntry(EntryId, comment);
        }
    }
}
