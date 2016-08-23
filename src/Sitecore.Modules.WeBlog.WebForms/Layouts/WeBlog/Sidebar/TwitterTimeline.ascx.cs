using System;
using Sitecore.Sharedsource.Web.UI.Sublayouts;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts.Sidebar
{
    public partial class BlogTwitter : SublayoutBase
    {
        public string Username { get; set; }
        public string WidgetId { get; set; }
        public int NumberOfTweets { get; set; }
        public string Theme { get; set; }
        public string BorderColour { get; set; }
        public string LinkColour { get; set; }
        public string Chrome { get; set; }
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