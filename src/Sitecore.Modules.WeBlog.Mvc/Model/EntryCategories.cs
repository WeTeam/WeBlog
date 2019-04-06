using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class EntryCategories: BlogRenderingModelBase
    {
        public CategoryItem[] CategoryItems { get; set; }

        public bool IsPageEditing
        {
            get
            {
                return Context.PageMode.IsExperienceEditorEditing;
            }
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            CategoryItems = ManagerFactory.CategoryManagerInstance.GetCategoriesByEntryID(CurrentEntry.ID);
        }
    }
}