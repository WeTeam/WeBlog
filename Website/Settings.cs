using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Configuration;

namespace Sitecore.Modules.WeBlog
{
    /// <summary>
    /// Provides access to blog settings from Sitecore settings
    /// </summary>
    [Obsolete("Use the WeBlogSettings class instead.")]
    public static class Settings
    {
        private static IWeBlogSettings _settings = new WeBlogSettings();

        /// <summary>
        /// Gets the name of the search index from Sitecore settings
        /// </summary>
        public static string SearchIndexName
        {
            get { return _settings.SearchIndexName; }
        }

        /// <summary>
        /// Gets the ID of the entry template from Sitecore settings as a string
        /// </summary>
        public static string EntryTemplateIDString
        {
            get { return EntryTemplateID.ToString(); }
        }

        /// <summary>
        /// Gets the ID of the entry template from Sitecore settings
        /// </summary>
        public static ID EntryTemplateID
        {
            get { return _settings.EntryTemplateIds.First(); }
        }

        /// <summary>
        /// Gets the ID of the comment template from Sitecore settings as a string
        /// </summary>
        public static string CommentTemplateIDString
        {
            get { return CommentTemplateID.ToString(); }
        }

        /// <summary>
        /// Gets the ID of the comment template from Sitecore settings
        /// </summary>
        public static ID CommentTemplateID
        {
            get { return _settings.CommentTemplateIds.First(); }
        }

        /// <summary>
        /// Gets the ID of the blog template from Sitecore settings as a string
        /// </summary>
        public static string BlogTemplateIDString
        {
            get { return BlogTemplateID.ToString(); }
        }

        /// <summary>
        /// Gets the ID of the blog template from Sitecore settings
        /// </summary>
        public static ID BlogTemplateID
        {
            
            get { return _settings.BlogTemplateIds.First(); }
        }

        /// <summary>
        /// Gets the ID of the category template from Sitecore settings as a string
        /// </summary>
        public static string CategoryTemplateIDString
        {
            get { return CategoryTemplateID.ToString(); }
        }

        /// <summary>
        /// Gets the ID of the category template from Sitecore settings
        /// </summary>
        public static ID CategoryTemplateID
        {
            get { return _settings.CategoryTemplateIds.First(); }
        }

        /// <summary>
        /// Gets the ID of the RSS Feed template from Sitecore settings as a string
        /// </summary>
        public static string RssFeedTemplateIDString
        {
            get { return RssFeedTemplateID.ToString(); }
        }

        /// <summary>
        /// Gets the ID of the RSS Feed template from Sitecore settings
        /// </summary>
        public static ID RssFeedTemplateID
        {
            get { return _settings.RssFeedTemplateIds.First(); }
        }

        /// <summary>
        /// Gets the ID of the blog branch from Sitecore settings as a string
        /// </summary>
        public static string BlogBranchIDString
        {
            get { return BlogBranchID.ToString(); }
        }

        /// <summary>
        /// Gets the ID of the blog branch from Sitecore settings
        /// </summary>
        public static ID BlogBranchID
        {
            get { return _settings.BlogBranchIds.First(); }
        }

        /// <summary>
        /// Gets the path to the root of the theme definition items from Sitecore settings
        /// </summary>
        public static string ThemesRoot
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.ThemesRoot", "/sitecore/system/Modules/WeBlog/Themes"); }
        }

        /// <summary>
        /// Gets the URL for the Gravatar image service from Sitecore settings
        /// </summary>
        public static string GravatarImageServiceUrl
        {
            get { return _settings.GravatarImageServiceUrl; }
        }

        /// <summary>
        /// Gets the reCAPTCHA private key from Sitecore settings
        /// </summary>
        public static string ReCaptchaPrivateKey
        {
            get { return _settings.ReCaptchaPrivateKey; }
        }

        /// <summary>
        /// Gets the reCAPTCHA public key from Sitecore settings
        /// </summary>
        public static string ReCaptchaPublicKey
        {
            get { return _settings.ReCaptchaPublicKey; }
        }

        /// <summary>
        /// Gets the AddThis account name from Sitecore settings
        /// </summary>
        public static string AddThisAccountName
        {
            get { return _settings.AddThisAccountName; }
        }

        /// <summary>
        /// Gets the ShareThis publisher ID from Sitecore settings
        /// </summary>
        public static string ShareThisPublisherID
        {
            get { return _settings.ShareThisPublisherId; }
        }

        /// <summary>
        /// Gets the Akismet API key from Sitecore settings
        /// </summary>
        public static string AkismetAPIKey
        {
            get { return _settings.AkismetAPIKey; }
        }

        /// <summary>
        /// Gets the content root path from Sitecore settings
        /// </summary>
        public static string ContentRootPath
        {
            get { return _settings.ContentRootPath; }
        }

        /// <summary>
        /// Gets the size for the globalization cache.
        /// </summary>
        public static string GlobalizationCacheSize
        {
            get { return _settings.GlobalizationCacheSize; }
        }

        /// <summary>
        /// Gets the default dictionary path.
        /// </summary>
        public static string DictionaryPath
        {
            get { return _settings.DictionaryPath; }
        }

        /// <summary>
        /// Gets the dictionary entry templateid as string.
        /// </summary>
        public static string DictionaryEntryTemplateIDString
        {
            get { return DictionaryEntryTemplateID.ToString(); }
        }

        /// <summary>
        /// Gets the dictionary entry template id.
        /// </summary>
        public static ID DictionaryEntryTemplateID
        {
            get { return _settings.DictionaryEntryTemplateId; }
        }

        /// <summary>
        /// Gets the maximum timeout period for the captcha control
        /// </summary>
        public static TimeSpan CaptchaMaximumTimeout
        {
            get { return _settings.CaptchaMaximumTimeout; }
        }

        /// <summary>
        /// Gets the minimum timeout period for the captcha control
        /// </summary>
        public static TimeSpan CaptchaMinimumTimeout
        {
            get { return _settings.CaptchaMinimumTimeout; }
        }

        /// <summary>
        /// Gets the ID of the workflow command to execute after creating a comment.
        /// </summary>
        public static string CommentWorkflowCommandCreated
        {
            get { return _settings.CommentWorkflowCommandCreated; }
        }

        /// <summary>
        /// Gets the ID of the workflow command to execute after a comment is classified as spam.
        /// </summary>
        public static string CommentWorkflowCommandSpam
        {
            get { return _settings.CommentWorkflowCommandSpam; }
        }

        /// <summary>
        /// Gets the ProfanityFilter file path
        /// </summary>
        public static string ProfanityFilterCacheSize
        {
            get { return _settings.ProfanityFilterCacheSize; }
        }

        public static string ProfanityListTemplateIDString
        {
            get { return ProfanityListTemplateID.ToString(); }
        }

        public static ID ProfanityListTemplateID
        {
            get { return _settings.ProfanityListTemplateId; }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the IBlogManager interface
        /// </summary>
        public static string BlogManagerClass
        {
            get { return _settings.BlogManagerClass; }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ICategoryManager interface
        /// </summary>
        public static string CategoryManagerClass
        {
            get { return _settings.CategoryManagerClass; }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ICommentManager interface
        /// </summary>
        public static string CommentManagerClass
        {
            get { return _settings.CommentManagerClass; }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the IEntryManager interface
        /// </summary>
        public static string EntryManagerClass
        {
            get { return _settings.EntryManagerClass; }
        }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ITagManager interface
        /// </summary>
        public static string TagManagerClass
        {
            get { return _settings.TagManagerClass; }
        }

        /// <summary>
        /// Gets the date format setting
        /// </summary>
        public static string DateFormat
        {
            get { return _settings.DateFormat; }
        }
    }
}