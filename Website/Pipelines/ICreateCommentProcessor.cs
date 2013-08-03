using System;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public interface ICreateCommentProcessor
    {
        void Process(CreateCommentArgs args);
    }
}