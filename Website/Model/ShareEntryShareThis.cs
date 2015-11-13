using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class ShareEntryShareThis:BlogRenderingModelBase
    {
        public string ShareThisPublisherId { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            ShareThisPublisherId = Settings.ShareThisPublisherID;
        }
    }
}