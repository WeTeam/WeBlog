using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Data;

namespace Sitecore.Modules.Eviblog.Services
{
    public class CommentService : ICommentService
    {
        public bool SubmitComment(ID EntryId, Model.Comment comment)
        {
            return CommentManager.AddCommentToEntry(EntryId, comment);
        }
    }
}
