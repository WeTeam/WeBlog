using System;
using Joel.Net;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sites;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class AkismetSpamCheck : ICreateCommentProcessor
    {
        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.CommentItem, "Comment Item cannot be null");
            Assert.IsNotNullOrEmpty(Settings.AkismetAPIKey, "Akismet API key must be set");

            if (!string.IsNullOrEmpty(Settings.CommentWorkflowCommandSpam))
            {
                var workflow = args.Database.WorkflowProvider.GetWorkflow(args.CommentItem);

                if (workflow != null)
                {
                    var api = new Akismet(Settings.AkismetAPIKey, ManagerFactory.BlogManagerInstance.GetCurrentBlog().Url, "WeBlog/2.1");
                    var isSpam = api.CommentCheck(args.CommentItem);

                    if (isSpam)
                    {
                        //Need to switch to shell website to execute workflow
                        using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext("shell")))
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