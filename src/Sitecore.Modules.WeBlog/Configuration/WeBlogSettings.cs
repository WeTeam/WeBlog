using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Configuration
{
    public class WeBlogSettings : IWeBlogSettings
    {
        /// <summary>
        /// Gets the name of the search index.
        /// </summary>
        public string SearchIndexName
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting("WeBlog.SearchIndexName", "WeBlog");
            }
        }

        /// <summary>
        /// Gets the IDs of the entry templates.
        /// </summary>
        public IEnumerable<ID> EntryTemplateIds
        {
            get
            {
                var ids = Sitecore.Configuration.Settings.GetSetting("WeBlog.EntryTemplateID", "{5FA92FF4-4AC2-48E2-92EB-E1E4914677B0}");
                return from id in ids.Split(new[] {'|'})
                    select ID.Parse(id);
            }
        }

        /// <summary>
        /// Gets the IDs of the comment templates.
        /// </summary>
        public IEnumerable<ID> CommentTemplateIds
        {
            get
            {
                var ids = Sitecore.Configuration.Settings.GetSetting("WeBlog.CommentTemplateID", "{70949D4E-35D8-4581-A7A2-52928AA119D5}");
                return from id in ids.Split(new[] { '|' })
                       select ID.Parse(id);
            }
        }

        /// <summary>
        /// Gets the IDs of the blog templates.
        /// </summary>
        public IEnumerable<ID> BlogTemplateIds
        {
            get
            {
                var ids = Sitecore.Configuration.Settings.GetSetting("WeBlog.BlogTemplateID", "{46663E05-A6B8-422A-8E13-36CD2B041278}");
                return from id in ids.Split(new[] { '|' })
                       select ID.Parse(id);
            }
        }

        /// <summary>
        /// Gets the IDs of the category templates.
        /// </summary>
        public IEnumerable<ID> CategoryTemplateIds
        {
            get
            {
                var ids = Sitecore.Configuration.Settings.GetSetting("WeBlog.CategoryTemplateID", "{61FF8D49-90D7-4E59-878D-DF6E03400D3B}");
                return from id in ids.Split(new[] { '|' })
                       select ID.Parse(id);
            }
        }

        /// <summary>
        /// Gets the IDs of the RSS Feed templates.
        /// </summary>
        public IEnumerable<ID> RssFeedTemplateIds
        {
            get
            {
                var ids = Sitecore.Configuration.Settings.GetSetting("WeBlog.RSSFeedTemplateID", "{B960CBE4-381F-4A2B-9F44-A43C7A991A0B}");
                return from id in ids.Split(new[] { '|' })
                       select ID.Parse(id);
            }
        }

        /// <summary>
        /// Gets the IDs of the blog branches
        /// </summary>
        public IEnumerable<ID> BlogBranchIds
        {
            get
            {
                var ids = Sitecore.Configuration.Settings.GetSetting("WeBlog.BlogBranchTemplateID", "{6FC4278C-E043-458B-9D5D-BBA775A9C386}");
                return from id in ids.Split(new[] { '|' })
                       select ID.Parse(id);
            }
        }

        /// <summary>
        /// Gets the URL for the Gravatar image service.
        /// </summary>
        public string GravatarImageServiceUrl
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Gravatar.ImageService.Url", "http://www.gravatar.com/avatar"); }
        }

        /// <summary>
        /// Gets the reCAPTCHA private key.
        /// </summary>
        public string ReCaptchaPrivateKey
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.reCAPTCHA.PrivateKey"); }
        }

        /// <summary>
        /// Gets the reCAPTCHA public key.
        /// </summary>
        public string ReCaptchaPublicKey
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.reCAPTCHA.PublicKey"); }
        }

        /// <summary>
        /// Gets the AddThis account name.
        /// </summary>
        public string AddThisAccountName
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.AddThisAccountName"); }
        }

        /// <summary>
        /// Gets the ShareThis publisher ID.
        /// </summary>
        public string ShareThisPublisherId
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.ShareThisPublisherID"); }
        }

        /// <summary>
        /// Gets the Akismet API key.
        /// </summary>
        public string AkismetAPIKey
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Akismet.APIKey"); }
        }

        /// <summary>
        /// Gets the content root path.
        /// </summary>
        public string ContentRootPath
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.ContentRootPath", "/sitecore/content"); }
        }

        /// <summary>
        /// Gets the size for the globalization cache.
        /// </summary>
        public string GlobalizationCacheSize
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Globalization.CacheSize", "500KB"); }
        }

        /// <summary>
        /// Gets the default dictionary path.
        /// </summary>
        public string DictionaryPath
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Globalization.DictonaryPath"); }
        }

        /// <summary>
        /// Gets the dictionary entry template id.
        /// </summary>
        public ID DictionaryEntryTemplateId
        {
            get
            {
                var id = Sitecore.Configuration.Settings.GetSetting("WeBlog.Globalization.DictonaryEntryTemplateId");
                return ID.Parse(id);
            }
        }

        /// <summary>
        /// Gets the maximum timeout period for the captcha control.
        /// </summary>
        public TimeSpan CaptchaMaximumTimeout
        {
            get { return Sitecore.Configuration.Settings.GetTimeSpanSetting("WeBlog.Captcha.MaxTimeout", "00:01:00"); }
        }

        /// <summary>
        /// Gets the minimum timeout period for the captcha control.
        /// </summary>
        public TimeSpan CaptchaMinimumTimeout
        {
            get { return Sitecore.Configuration.Settings.GetTimeSpanSetting("WeBlog.Captcha.MinTimeout", "00:00:03"); }
        }

        /// <summary>
        /// Gets the ID of the workflow command to execute after creating a comment.
        /// </summary>
        public string CommentWorkflowCommandCreated
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Comments.Workflow.Command.Created", ""); }
        }

        /// <summary>
        /// Gets the ID of the workflow command to execute after a comment is classified as spam.
        /// </summary>
        public string CommentWorkflowCommandSpam
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Comments.Workflow.Command.Spam"); }
        }

        /// <summary>
        /// Gets the cache size for the profanity filter.
        /// </summary>
        public string ProfanityFilterCacheSize
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.ProfanityFilter.CacheSize"); }
        }

        /// <summary>
        /// Gets the template ID for the profanity list.
        /// </summary>
        public ID ProfanityListTemplateId
        {
            get
            {
                var id = Sitecore.Configuration.Settings.GetSetting("WeBlog.ProfanityFilter.ProfanityListTemplateID");
                return ID.Parse(id);
            }
        }

        /// <summary>
        /// Gets the date format setting.
        /// </summary>
        public string DateFormat
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.DateFormat", "MMMM dd yyyy"); }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the IBlogManager interface.
        /// </summary>
        public string BlogManagerClass
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Implementation.BlogManager"); }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ICategoryManager interface.
        /// </summary>
        public string CategoryManagerClass
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Implementation.CategoryManager"); }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ICommentManager interface.
        /// </summary>
        public string CommentManagerClass
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Implementation.CommentManager"); }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the IEntryManager interface.
        /// </summary>
        public string EntryManagerClass
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Implementation.EntryManager"); }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ITagManager interface.
        /// </summary>
        public string TagManagerClass
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Implementation.TagManager"); }
        }

        /// <summary>
        /// Indicates whether to use the comment service or not.
        /// </summary>
        public bool CommentServiceEnabled
        {
            get { return Sitecore.Configuration.Settings.GetBoolSetting("WeBlog.CommentService.Enable", false); }
        }
    }
}