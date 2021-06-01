using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class WorkflowSubmit : ICreateCommentProcessor
    {
        private readonly IWeBlogSettings _settings;
        private readonly IWeBlogCommentSettings _commentSettings = null;

        public WorkflowSubmit()
            : this(WeBlogSettings.Instance, null)
        {
        }

        public WorkflowSubmit(IWeBlogSettings settings, IWeBlogCommentSettings commentSettings)
        {
            _settings = settings;
            _commentSettings = commentSettings ?? ServiceLocator.ServiceProvider.GetRequiredService<IWeBlogCommentSettings>();
        }

        public void Process(CreateCommentArgs args)
        {
            if (!string.IsNullOrEmpty(_commentSettings.CommentWorkflowCommandCreated))
            {
                var workflow = args.Database.WorkflowProvider.GetWorkflow(args.CommentItem);

                if (workflow != null)
                {
                    //Need to switch to shell website to execute workflow
                    using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext(Sitecore.Constants.ShellSiteName)))
                    {
                        workflow.Execute(_commentSettings.CommentWorkflowCommandCreated, args.CommentItem, "WeBlog automated submit", false, new object[0]);
                    }
                    //workflow should take care of publishing the item following this, if it's configured to do so
                }
            }
        }
    }
}