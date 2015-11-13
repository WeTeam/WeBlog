using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface ICommentsListCore
    {
        CommentItem[] LoadComments(CommentItem addedComment = null);
        string GetGravatarUrl(string text);
    }
}