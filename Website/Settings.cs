using System;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog
{
    /// <summary>
    /// Provides access to blog settings from Sitecore settings
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Gets the name of the search index from Sitecore settings
        /// </summary>
        public static string SearchIndexName
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.SearchIndexName", "WeBlog"); }
        }

        /// <summary>
        /// Gets the ID of the entry template from Sitecore settings as a string
        /// </summary>
        public static string EntryTemplateIdString
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.EntryTemplateID", "{5FA92FF4-4AC2-48E2-92EB-E1E4914677B0}"); }
        }

        /// <summary>
        /// Gets the ID of the entry template from Sitecore settings
        /// </summary>
        public static ID EntryTemplateId
        {
            get
            {
                var id = ID.Null;
                ID.TryParse(EntryTemplateIdString, out id);
                return id;
            }
        }

        /// <summary>
        /// Gets the ID of the comment template from Sitecore settings as a string
        /// </summary>
        public static string CommentTemplateIdString
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.CommentTemplateID", "{70949D4E-35D8-4581-A7A2-52928AA119D5}"); }
        }

        /// <summary>
        /// Gets the ID of the comment template from Sitecore settings
        /// </summary>
        public static ID CommentTemplateId
        {
            get
            {
                var id = ID.Null;
                ID.TryParse(CommentTemplateIdString, out id);
                return id;
            }
        }

        /// <summary>
        /// Gets the ID of the blog template from Sitecore settings as a string
        /// </summary>
        public static string BlogTemplateIdString
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.BlogTemplateID", "{46663E05-A6B8-422A-8E13-36CD2B041278}"); }
        }

        /// <summary>
        /// Gets the ID of the blog template from Sitecore settings
        /// </summary>
        public static ID BlogTemplateId
        {
            get
            {
                var id = ID.Null;
                ID.TryParse(BlogTemplateIdString, out id);
                return id;
            }
        }

        /// <summary>
        /// Gets the ID of the category template from Sitecore settings as a string
        /// </summary>
        public static string CategoryTemplateIdString
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.CategoryTemplateID", "{61FF8D49-90D7-4E59-878D-DF6E03400D3B}"); }
        }

        /// <summary>
        /// Gets the ID of the category template from Sitecore settings
        /// </summary>
        public static ID CategoryTemplateId
        {
            get
            {
                var id = ID.Null;
                ID.TryParse(CategoryTemplateIdString, out id);
                return id;
            }
        }

        /// <summary>
        /// Gets the ID of the RSS Feed template from Sitecore settings as a string
        /// </summary>
        public static string RssFeedTemplateIdString
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.RSSFeedTemplateID", "{B960CBE4-381F-4A2B-9F44-A43C7A991A0B}"); }
        }

        /// <summary>
        /// Gets the ID of the RSS Feed template from Sitecore settings
        /// </summary>
        public static ID RssFeedTemplateId
        {
            get
            {
                var id = ID.Null;
                ID.TryParse(RssFeedTemplateIdString, out id);
                return id;
            }
        }

        /// <summary>
        /// Gets the ID of the blog branch from Sitecore settings as a string
        /// </summary>
        public static string BlogBranchIdString
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.BlogBranchTemplateID", "{6FC4278C-E043-458B-9D5D-BBA775A9C386}"); }
        }

        /// <summary>
        /// Gets the ID of the blog branch from Sitecore settings
        /// </summary>
        public static ID BlogBranchId
        {
            get
            {
                var id = ID.Null;
                ID.TryParse(BlogBranchIdString, out id);
                return id;
            }
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
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Gravatar.ImageService.Url", "http://www.gravatar.com/avatar"); }
        }

        /// <summary>
        /// Gets the reCAPTCHA private key from Sitecore settings
        /// </summary>
        public static string ReCaptchaPrivateKey
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.reCAPTCHA.PrivateKey"); }
        }

        /// <summary>
        /// Gets the reCAPTCHA public key from Sitecore settings
        /// </summary>
        public static string ReCaptchaPublicKey
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.reCAPTCHA.PublicKey"); }
        }

        /// <summary>
        /// Gets the AddThis account name from Sitecore settings
        /// </summary>
        public static string AddThisAccountName
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.AddThisAccountName"); }
        }

        /// <summary>
        /// Gets the ShareThis publisher ID from Sitecore settings
        /// </summary>
        public static string ShareThisPublisherID
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.ShareThisPublisherID"); }
        }

        /// <summary>
        /// Gets the Akismet API key from Sitecore settings
        /// </summary>
        public static string AkismetAPIKey
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Akismet.APIKey"); }
        }

        /// <summary>
        /// Gets the content root path from Sitecore settings
        /// </summary>
        public static string ContentRootPath
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.ContentRootPath", "/sitecore/content"); }
        }

        /// <summary>
        /// Gets the size for the globalization cache.
        /// </summary>
        public static string GlobalizationCacheSize
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Globalization.CacheSize"); }
        }

        /// <summary>
        /// Gets the default dictionary path.
        /// </summary>
        public static string DictionaryPath
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Globalization.DictonaryPath"); }
        }

        /// <summary>
        /// Gets the dictionary entry templateid as string.
        /// </summary>
        public static string DictionaryEntryTemplateIdString
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Globalization.DictonaryEntryTemplateId"); }
        }

        /// <summary>
        /// Gets the dictionary entry template id.
        /// </summary>
        public static ID DictionaryEntryTemplateId
        {
            get
            {
                var id = ID.Null;
                ID.TryParse(DictionaryEntryTemplateIdString, out id);
                return id;
            }
        }

        /// <summary>
        /// Gets the maximum timeout period for the captcha control
        /// </summary>
        public static TimeSpan CaptchaMaximumTimeout
        {
            get { return Sitecore.Configuration.Settings.GetTimeSpanSetting("WeBlog.Captcha.MaxTimeout", "00:01:00"); }
        }

        /// <summary>
        /// Gets the minimum timeout period for the captcha control
        /// </summary>
        public static TimeSpan CaptchaMinimumTimeout
        {
            get { return Sitecore.Configuration.Settings.GetTimeSpanSetting("WeBlog.Captcha.MinTimeout", "00:00:03"); }
        }

        /// <summary>
        /// Gets the ID of the workflow command to execute after creating a comment.
        /// </summary>
        public static string CommentWorkflowCommandCreated
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Comments.Workflow.Command.Created"); }
        }

        /// <summary>
        /// Gets the ID of the workflow command to execute after a comment is classified as spam.
        /// </summary>
        public static string CommentWorkflowCommandSpam
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Comments.Workflow.Command.Spam"); }
        }
    }
}