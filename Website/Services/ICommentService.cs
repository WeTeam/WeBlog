using System;
using System.ServiceModel;
using Sitecore.Data;

namespace Sitecore.Modules.Blog.Services
{
    [ServiceContract]
    public interface ICommentService
    {
        [OperationContract]
        ID SubmitComment(ID EntryId, Model.Comment comment);
    }
}
