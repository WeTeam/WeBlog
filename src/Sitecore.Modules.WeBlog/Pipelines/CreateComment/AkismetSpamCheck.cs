using Joel.Net;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sites;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class AkismetSpamCheck : ICreateCommentProcessor
    {
        private readonly IWeBlogCommentSettings _commentSettings = null;
        private readonly IAkismet _akismetApi = null;
        private readonly IBlogManager _blogManager = null;
        private readonly BaseLinkManager _linkManager = null;

        public AkismetSpamCheck()
            : this(commentSettings: null, blogManager: null, akismetApi: null, linkManager: null)
        {
        }

        [Obsolete("Use ctor(IWeBlogCommentSettings, IBlogManager, IAkismet, BaseLinkManager) instead.")]
        public AkismetSpamCheck(IWeBlogSettings settings, IBlogManager blogManager, IAkismet akismetApi, BaseLinkManager linkManager)
            : this(commentSettings: null, blogManager: null, akismetApi: null, linkManager: null)
        {

        }

        public AkismetSpamCheck(IWeBlogCommentSettings commentSettings, IBlogManager blogManager, IAkismet akismetApi, BaseLinkManager linkManager)
        {
            _commentSettings = commentSettings ?? ServiceLocator.ServiceProvider.GetRequiredService<IWeBlogCommentSettings>();
            _blogManager = blogManager ?? ManagerFactory.BlogManagerInstance;
            _akismetApi = akismetApi;
            _linkManager = linkManager ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseLinkManager>();
        }

        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.CommentItem, "Comment Item cannot be null");

            if (!string.IsNullOrEmpty(_commentSettings.AkismetAPIKey) && !string.IsNullOrEmpty(_commentSettings.CommentWorkflowCommandSpam))
            {
                var workflow = args.Database.WorkflowProvider.GetWorkflow(args.CommentItem);

                if (workflow != null)
                {
                    var api = _akismetApi;
                    if(api == null)
                        api = new Akismet();

                    var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

                    var blogItem = _blogManager.GetCurrentBlog(args.CommentItem);
                    var url = _linkManager.GetAbsoluteItemUrl(blogItem);

                    api.Init(_commentSettings.AkismetAPIKey, url, "WeBlog/" + version);

                    var isSpam = api.CommentCheck(args.CommentItem);

                    if (isSpam)
                    {
                        //Need to switch to shell website to execute workflow
                        using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext(Sitecore.Constants.ShellSiteName)))
                        {
                            workflow.Execute(_commentSettings.CommentWorkflowCommandSpam, args.CommentItem, "Akismet classified this comment as spam", false, new object[0]);
                        }

                        args.AbortPipeline();
                    }
                }
            }
        }
    }
}