using System;
using System.Linq;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogEntryTags : BaseEntrySublayout
    {
        public IEntryTagsCore EntryTagsCore { get; set; }

        public BlogEntryTags(IEntryTagsCore entryTagsCore = null)
        {
            EntryTagsCore = entryTagsCore ?? new EntryTagsCore(CurrentBlog);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadEntry();
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        protected virtual void LoadEntry()
        {
            // Load tags
#if !FEATURE_EXPERIENCE_EDITOR
            if (!Sitecore.Context.PageMode.IsPageEditorEditing)
#else
            if (!Sitecore.Context.PageMode.IsExperienceEditorEditing)
#endif
            {
                var tags = ManagerFactory.TagManagerInstance.GetTagsByEntry(CurrentEntry);
                var list = LoginViewTags.FindControl("TagList") as ListView;

                if (list != null)
                {
                    list.DataSource = from tag in tags select tag.Key;
                    list.DataBind();
                }
            }
        }
    }
}