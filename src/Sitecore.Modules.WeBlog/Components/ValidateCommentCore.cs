using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Pipelines.ValidateComment;
using System.Collections.Specialized;

namespace Sitecore.Modules.WeBlog.Components
{
    public class ValidateCommentCore : IValidateCommentCore
    {
        protected BaseCorePipelineManager CorePipelineManager { get; }

        public ValidateCommentCore(BaseCorePipelineManager corePipelineManager)
        {
            CorePipelineManager = corePipelineManager ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseCorePipelineManager>();
        }

        public CommentValidationResult Validate(Comment comment, NameValueCollection form)
        {
            var args = new ValidateCommentArgs(comment, form);
            CorePipelineManager.Run("weblogValidateComment", args, true);

            return new CommentValidationResult(args.Errors);
        }
    }
}
