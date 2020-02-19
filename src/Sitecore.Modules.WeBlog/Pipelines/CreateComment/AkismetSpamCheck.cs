using System;
using System.Diagnostics;
using System.Reflection;
using Joel.Net;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Sites;

#if FEATURE_ABSTRACTIONS
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
#endif

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class AkismetSpamCheck : ICreateCommentProcessor
    {
        private IWeBlogSettings _settings = null;
        private IAkismet _akismetApi = null;
        private IBlogManager _blogManager = null;

#if FEATURE_ABSTRACTIONS
        private BaseLinkManager _linkManager = null;
#endif

        public AkismetSpamCheck()
            : this(null, null, null
#if FEATURE_ABSTRACTIONS
                  , null
#endif
                  )
        {
        }

        [Obsolete("Use ctor(IWeBlogSettings, BaseLinkManager, IBlogManager, IAkismet) instead.")]
        public AkismetSpamCheck(IWeBlogSettings settings)
            : this(settings, null, null
#if FEATURE_ABSTRACTIONS
                  , null
#endif
                  )
        {
        }

        public AkismetSpamCheck(IWeBlogSettings settings, IBlogManager blogManager, IAkismet akismetApi
#if FEATURE_ABSTRACTIONS
            , BaseLinkManager linkManager
#endif

            )
        {
            _settings = settings ?? WeBlogSettings.Instance;
            _blogManager = blogManager ?? ManagerFactory.BlogManagerInstance;
            _akismetApi = akismetApi;

#if FEATURE_ABSTRACTIONS
            _linkManager = linkManager ?? ServiceLocator.ServiceProvider.GetService(typeof(BaseLinkManager)) as BaseLinkManager;
#endif
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

#if FEATURE_ABSTRACTIONS
                    var url = _linkManager.GetItemUrl(_blogManager.GetCurrentBlog(), urlOptions);
#else
                    var url = LinkManager.GetItemUrl(_blogManager.GetCurrentBlog(), urlOptions);
#endif

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