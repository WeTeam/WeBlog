using Sitecore.Data;
using Sitecore.Modules.WeBlog.Managers;
using System.Runtime.Serialization;

namespace Sitecore.Modules.WeBlog.Services
{
#if PRE_65
    [KnownType(typeof(System.Collections.CaseInsensitiveHashCodeProvider))]
#endif
    public class CommentService : ICommentService
    {
        public ID SubmitComment(ID EntryId, Model.Comment comment)
        {
            return ManagerFactory.CommentManagerInstance.AddCommentToEntry(EntryId, comment);
        }
    }
}
