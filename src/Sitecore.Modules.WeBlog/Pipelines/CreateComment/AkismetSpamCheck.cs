using System;
using System.Diagnostics;
using System.Reflection;
using Joel.Net;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class AkismetSpamCheck : ICreateCommentProcessor
    {
        private IWeBlogSettings _settings = null;
        private IAkismet _akismetApi = null;
        private IBlogManager _blogManager = null;
        private BaseLinkManager _linkManager = null;

        public AkismetSpamCheck()
            : this(null, null, null, null)
        {
        }

        [Obsolete("Use ctor(IWeBlogSettings, BaseLinkManager, IBlogManager, IAkismet) instead.")]
        public AkismetSpamCheck(IWeBlogSettings settings)
            : this(settings, null, null, null)
        {
        }

        public AkismetSpamCheck(IWeBlogSettings settings, IBlogManager blogManager, IAkismet akismetApi, BaseLinkManager linkManager)
        {
            _settings = settings ?? WeBlogSettings.Instance;
            _blogManager = blogManager ?? ManagerFactory.BlogManagerInstance;
            _akismetApi = akismetApi;
            _linkManager = linkManager ?? ServiceLocator.ServiceProvider.GetService(typeof(BaseLinkManager)) as BaseLinkManager;
        }

        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.CommentItem, "Comment Item cannot be null");

            if (!string.IsNullOrEmpty(_settings.AkismetAPIKey) && !string.IsNullOrEmpty(_settings.CommentWorkflowCommandSpam))
            {
                var workflow = args.Database.WorkflowProvider.GetWorkflow(args.CommentItem);

                if (workflow != null)
                {
                    var api = _akismetApi;
                    if(api == null)
                        api = new Akismet();

                    var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

                    var urlOptions = UrlOptions.DefaultOptions;
                    urlOptions.AlwaysIncludeServerUrl = true;

                    var url = _linkManager.GetItemUrl(_blogManager.GetCurrentBlog(), urlOptions);

                    api.Init(_settings.AkismetAPIKey, url, "WeBlog/" + version);

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