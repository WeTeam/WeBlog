using System.ServiceModel;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Services
{
    [ServiceContract]
    public interface ICommentService
    {
        [OperationContract]
        ID SubmitComment(ID EntryId, Model.Comment comment);
    }
}
