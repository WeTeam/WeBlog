using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class ShareEntryAddThis:BlogRenderingModelBase
    {
        public string AddThisAccountName { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            var addThisAccountName = Settings.AddThisAccountName;
            AddThisAccountName = string.IsNullOrEmpty(addThisAccountName) ? "" : "#pubid=" + addThisAccountName;
        }
    }
}