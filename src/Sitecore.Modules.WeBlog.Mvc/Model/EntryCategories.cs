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
                return
#if !FEATURE_EXPERIENCE_EDITOR
                    Context.PageMode.IsPageEditorEditing;
#else
                    Context.PageMode.IsExperienceEditorEditing;
#endif
            }
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            CategoryItems = ManagerFactory.CategoryManagerInstance.GetCategoriesByEntryID(CurrentEntry.ID);
        }
    }
}