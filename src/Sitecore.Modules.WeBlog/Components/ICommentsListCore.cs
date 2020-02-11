using System.Collections.Generic;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface ICommentsListCore
    {
        IList<CommentContent> LoadComments(CommentItem addedComment = null);

        string GetGravatarUrl(string text);
    }
}