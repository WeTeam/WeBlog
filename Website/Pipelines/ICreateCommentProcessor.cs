namespace Sitecore.Modules.WeBlog.Pipelines
{
    public interface ICreateCommentProcessor
    {
        void Process(CreateCommentArgs args);
    }
}