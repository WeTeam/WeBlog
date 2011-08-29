using System;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog
{
    public static class Constants
    {
        public static class Index
        {
            public static class Fields
            {
                public const string BlogID = "blogid";
                public const string EntryID = "entryid";
                public const string Tags = "tags";
                public const string Category = "category";
                public const string Created = "__created";
            }
        }

        public static class Fields
        {
            public const string IpAddress = "IP Address";
            public const string Website = "Website";
        }

        public static class Templates
        {
            public static class DictionaryEntry
            {
                public static ID ID = ID.Parse("{819F1459-A930-45E4-975B-9165028E6A58}");
            }
        }
    }
}