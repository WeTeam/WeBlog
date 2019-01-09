﻿using System;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog
{
    public static class Constants
    {
        public const string CookieName = "weblog-comment-submissions";

        public static class Index
        {
            public static class Fields
            {
                public const string BlogID = "blogid";
                public const string EntryID = "entryid";
                public const string Tags = "tags";
                public const string Category = "category";
                public const string Created = "_created";
                public const string Template = "template";
                public const string Language = "language";
                public const string EntryDate = "entry date";
            }
        }

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

#if FEATURE_XDB
        public static class ReportingQueries
        {
            public static readonly ID ItemVisits = new ID("{9BECF823-2838-467C-87A7-3664524F5AFE}");
        }
#endif
        public class Tokens
        {
            public static string WeBlogSetting = "$weblogsetting";
            public static string WeBlogContext = "$weblogcontext";
        }
    }
}