namespace Sitecore.Modules.WeBlog.Model
{
    public class TwitterTimelineRenderingModel
    {
        public string Username { get; set; }
        public string WidgetId { get; set; }

        public int TweetLimit { get; set; }
        public string Hashtags { get; set; }

        public string Theme { get; set; }
        public string BorderColor { get; set; }
        public string LinkColor { get; set; }
        public string Chrome { get; set; }

        public string Width { get; set; }
        public string Height { get; set; }
    }
}