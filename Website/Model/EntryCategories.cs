using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class EntryCategories: BlogRenderingModelBase
    {
        public CategoryItem[] CategoryItems { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            CategoryItems = ManagerFactory.CategoryManagerInstance.GetCategoriesByEntryID(CurrentEntry.ID);
        }
    }
}