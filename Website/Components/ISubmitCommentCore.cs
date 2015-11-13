using Sitecore.Data;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Components
{
    public interface ISubmitCommentCore
    {
        ID Submit(Comment comment);
    }
}