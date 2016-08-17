using Joel.Net;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class AkismetSpamCheck : ICreateCommentProcessor
    {
        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.CommentItem, "Comment Item cannot be null");

            if (!string.IsNullOrEmpty(Settings.AkismetAPIKey) && !string.IsNullOrEmpty(Settings.CommentWorkflowCommandSpam))
            {
                var workflow = args.Database.WorkflowProvider.GetWorkflow(args.CommentItem);

                if (workflow != null)
                {
                    var api = new Akismet(Settings.AkismetAPIKey, ManagerFactory.BlogManagerInstance.GetCurrentBlog().SafeGet(x => x.Url), "WeBlog/2.1");
                    var isSpam = api.CommentCheck(args.CommentItem);

                    if (isSpam)
                    {
                        //Need to switch to shell website to execute workflow
                        using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext(Sitecore.Constants.ShellSiteName)))
                        {
                            workflow.Execute(Settings.CommentWorkflowCommandSpam, args.CommentItem, "Akismet classified this comment as spam", false, new object[0]);
                        }

                        args.AbortPipeline();
                    }
                }
            }
        }
    }
}