using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Components.EntryTags;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogEntryTagsController : BlogBaseController
    {
        protected IEntryTagsCore EntryTagsCore { get; set; }

        public BlogEntryTagsController() : this(null) { }

        public BlogEntryTagsController(IEntryTagsCore entryTagsCore)
        {
            EntryTagsCore = entryTagsCore ?? new EntryTagsCore(CurrentBlog);
        }

        public ActionResult Index()
        {
            Dictionary<string, string> model = new Dictionary<string, string>();
            if (!Context.PageMode.IsPageEditorEditing)
            {
                var tags = ManagerFactory.TagManagerInstance.GetTagsByEntry(CurrentEntry);
                model = tags.ToDictionary(t => t.Key, t => EntryTagsCore.GetTagUrl(t.Key));
            }
            return View("~/Views/WeBlog/EntryTags.cshtml", model);
        }
    }
}