using System;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Components
{
    public class SubmitCommentCore : ISubmitCommentCore
    {
        public ID Submit(Comment comment)
        {
            comment = NormalizeWebsiteAddress(comment);
            return ManagerFactory.CommentManagerInstance.SubmitComment(Context.Item.ID, comment, Context.Language);
        }

        private Comment NormalizeWebsiteAddress(Comment comment)
        {
            if (comment.Fields.ContainsKey(Constants.Fields.Website))
            {
                var website = comment.Fields[Constants.Fields.Website];
                comment.Fields[Constants.Fields.Website] = FixWebsiteAddress(website);
            }
            return comment;
        }

        private string FixWebsiteAddress(string website)
        {
            if (!String.IsNullOrEmpty(website))
            {
                website = website.Contains("://") ? website : String.Format("//{0}", website);
            }
            return website;
        }
    }
}