using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class Twitter : BlogRenderingModelBase
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

        public bool IsPageEditing
        {
            get
            {
                return Context.PageMode.IsExperienceEditor;
            }
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            if (string.IsNullOrEmpty(Width))
            {
                Width = "auto";
            }

            if (string.IsNullOrEmpty(Height))
            {
                Height = "auto";
            }
        }
    }
}