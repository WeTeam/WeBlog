using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class EntryTags: BlogRenderingModelBase
    {
        protected IEntryTagsCore EntryTagsCore { get; set; }

        public EntryTags() : this(null) { }

        public EntryTags(IEntryTagsCore entryTagsCore)
        {
            var blogHome = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            EntryTagsCore = entryTagsCore ?? new EntryTagsCore(blogHome);
            TagLinks = new Dictionary<string, string>();
        }

        public Dictionary<string, string> TagLinks { get; set; }

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

            if (Context.PageMode.IsExperienceEditorEditing)
                return;
            
            Tag[] tags = ManagerFactory.TagManagerInstance.GetTagsForEntry(rendering.Item);
            TagLinks = tags.ToDictionary(t => t.Name, t => EntryTagsCore.GetTagUrl(t.Name));
        }
    }
}