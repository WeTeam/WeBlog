using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class ProfanityFilter : ICreateCommentProcessor
    {
        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.Comment, "Comment cannot be null");
            ProfanityFilterArgs filterArgs = new ProfanityFilterArgs
            {
                Input = args.Comment.Text
            };

            CorePipeline.Run("weblogProfanityFilter", filterArgs);
            if (!filterArgs.Valid)
            {
                args.AbortPipeline();
            }
            args.Comment.Text = filterArgs.Input;
        }
    }
}