using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class ShareEntryShareThis : BlogRenderingModelBase
    {
        private readonly IWeBlogSettings _settings;

        public string ShareThisPublisherId { get; set; }

        public ShareEntryShareThis()
            : this(WeBlogSettings.Instance)
        {
        }

        public ShareEntryShareThis(IWeBlogSettings settings)
        {
            _settings = settings;
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            ShareThisPublisherId = _settings.ShareThisPublisherId;
        }
    }
}