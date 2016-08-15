using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class Theme : BlogRenderingModelBase
    {        
        public ThemeItem ThemeItem { get; protected set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);

            ThemeItem = CurrentBlog.Theme.Item;
        }
    }
}