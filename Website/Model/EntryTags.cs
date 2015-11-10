using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Components.EntryTags;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class EntryTags: BlogRenderingModelBase
    {
        protected IEntryTagsCore EntryTagsCore { get; set; }

        public EntryTags() : this(null) { }

        public EntryTags(IEntryTagsCore entryTagsCore)
        {
            var blogHome = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            EntryTagsCore = entryTagsCore ?? new EntryTagsCore(blogHome);
        }

        public Dictionary<string, string> TagLinks { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);

            if (Context.PageMode.IsPageEditorEditing)
            {
                return;
            }
            var tags = ManagerFactory.TagManagerInstance.GetTagsByEntry(rendering.Item);
            TagLinks = tags.ToDictionary(t => t.Key, t => EntryTagsCore.GetTagUrl(t.Key));
        }
    }
}