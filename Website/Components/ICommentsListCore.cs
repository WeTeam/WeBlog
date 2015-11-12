using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface ICommentsListCore
    {
        CommentItem[] LoadComments(CommentItem addedComment = null);
        string GetGravatarUrl(string text);
    }
}