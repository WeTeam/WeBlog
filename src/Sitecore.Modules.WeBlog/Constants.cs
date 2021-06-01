using Sitecore.Data;

namespace Sitecore.Modules.WeBlog
{
    public static class Constants
    {
        public const string CookieName = "weblog-comment-submissions";

        public static class Fields
        {
            public const string IpAddress = "IP Address";
            public const string Website = "Website";
            public const string WordList = "WordList";
        }

        public static class Templates
        {
            public static class DictionaryEntry
            {
                public static ID ID = ID.Parse("{819F1459-A930-45E4-975B-9165028E6A58}");
            }
        }

        public static class Events
        {
            public static class UI
            {
                public const string COMMENT_ADDED = "weblog:comment:added";
            }
        }

        public static class Paths
        {
            public const string WeBlogMedia = "/sitecore/media library/Modules/WeBlog";
            public const string PublishingTargets = "/sitecore/system/publishing targets";
        }

        public static class ReportingQueries
        {
            public static readonly ID ItemVisits = new ID("{9BECF823-2838-467C-87A7-3664524F5AFE}");
        }

        public static class TranslationPhrases
        {
            public static readonly string RequiredField = "REQUIRED_FIELD";
            public static readonly string Name = "NAME";
            public static readonly string Email = "EMAIL";
            public static readonly string Comment = "COMMENT";
            public static readonly string ErrorOccurredTryAgain = "ERROR_OCCURRED_TRY_AGAIN";
        }
    }
}