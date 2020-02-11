using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class WorkflowSubmit : ICreateCommentProcessor
    {
        private readonly IWeBlogSettings _settings;

        public WorkflowSubmit()
            : this(WeBlogSettings.Instance)
        {
        }

        public WorkflowSubmit(IWeBlogSettings settings)
        {
            _settings = settings;
        }

        public void Process(CreateCommentArgs args)
        {
            if (!string.IsNullOrEmpty(_settings.CommentWorkflowCommandCreated))
            {
                var workflow = args.Database.WorkflowProvider.GetWorkflow(args.CommentItem);

                if (workflow != null)
                {
                    //Need to switch to shell website to execute workflow
                    using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext(Sitecore.Constants.ShellSiteName)))
                    {
                        workflow.Execute(_settings.CommentWorkflowCommandCreated, args.CommentItem, "WeBlog automated submit", false, new object[0]);
                    }
                    //workflow should take care of publishing the item following this, if it's configured to do so
                }
            }
        }
    }
}