using Sitecore.Data;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Globalization;

namespace Sitecore.Modules.WeBlog.Services
{
    public class CommentService : ICommentService
    {
        public ID SubmitComment(ID EntryId, Model.Comment comment, Language language)
        {
            return ManagerFactory.CommentManagerInstance.AddCommentToEntry(EntryId, comment, language);
        }
    }
}
