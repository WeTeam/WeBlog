using System;
using System.ComponentModel;
using Sitecore.Sharedsource.Web.UI.Sublayouts;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogTwitter : SublayoutBase
    {
        public string Username { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool Polling { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool Scrollbar { get; set; }

        public int NumberOfTweets { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool Avatars { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool Timestamps { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool Hashtags { get; set; }

        public string ShellBackground { get; set; }
        public string ShellText { get; set; }
        public string TweetBackground { get; set; }
        public string TweetText { get; set; }
        public string Links { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (string.IsNullOrEmpty(Width))
                Width = "auto";

            if (string.IsNullOrEmpty(Height))
                Height = "auto";
        }
    }
}