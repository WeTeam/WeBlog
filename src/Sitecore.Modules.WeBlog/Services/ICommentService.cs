using System;
using System.ServiceModel;
using Sitecore.Data;
using Sitecore.Globalization;

namespace Sitecore.Modules.WeBlog.Services
{
    [ServiceContract]
    [Obsolete("Use the EventQueue for comment submission instead.")]
    public interface ICommentService
    {
        [OperationContract]
        ID SubmitComment(ID EntryId, Model.Comment comment, Language language);
    }
}
