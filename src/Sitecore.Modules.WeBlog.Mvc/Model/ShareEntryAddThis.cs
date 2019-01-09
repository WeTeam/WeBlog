using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class ShareEntryAddThis : BlogRenderingModelBase
    {
        private readonly IWeBlogSettings _settings;

        public string AddThisAccountName { get; set; }

        public ShareEntryAddThis()
            : this(WeBlogSettings.Instance)
        {
        }

        public ShareEntryAddThis(IWeBlogSettings settings)
        {
            _settings = settings;
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            var addThisAccountName = _settings.AddThisAccountName;
            AddThisAccountName = string.IsNullOrEmpty(addThisAccountName) ? "" : "#pubid=" + addThisAccountName;
        }
    }
}