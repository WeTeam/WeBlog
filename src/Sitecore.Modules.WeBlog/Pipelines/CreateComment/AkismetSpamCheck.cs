using System.Reflection;
using Joel.Net;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class AkismetSpamCheck : ICreateCommentProcessor
    {
        private IWeBlogSettings _settings;

        public AkismetSpamCheck()
            : this(WeBlogSettings.Instance)
        {
        }

        public AkismetSpamCheck(IWeBlogSettings settings)
        {
            _settings = settings;
        }

        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.CommentItem, "Comment Item cannot be null");

            if (!string.IsNullOrEmpty(_settings.AkismetAPIKey) && !string.IsNullOrEmpty(_settings.CommentWorkflowCommandSpam))
            {
                var workflow = args.Database.WorkflowProvider.GetWorkflow(args.CommentItem);

                if (workflow != null)
                {
                    var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyVersionAttribute>();
                    var api = new Akismet(_settings.AkismetAPIKey, ManagerFactory.BlogManagerInstance.GetCurrentBlog().SafeGet(x => x.Url), "WeBlog/" + version);
                    var isSpam = api.CommentCheck(args.CommentItem);

                    if (isSpam)
                    {
                        //Need to switch to shell website to execute workflow
                        using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext(Sitecore.Constants.ShellSiteName)))
                        {
                            workflow.Execute(_settings.CommentWorkflowCommandSpam, args.CommentItem, "Akismet classified this comment as spam", false, new object[0]);
                        }

                        args.AbortPipeline();
                    }
                }
            }
        }
    }
}