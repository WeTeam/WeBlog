using System;
using System.ServiceModel;
using Sitecore.Data;

namespace Sitecore.Modules.Eviblog.Services
{
    [ServiceContract]
    public interface ICommentService
    {
        [OperationContract]
        bool SubmitComment(ID EntryId, Model.Comment comment);
    }
}
