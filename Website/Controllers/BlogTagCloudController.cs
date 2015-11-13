using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogTagCloudController : BlogBaseController
    {
        protected ITagCloudCore TagCloudCore { get; set; }

        public BlogTagCloudController() : this(null) { }

        public BlogTagCloudController(ITagCloudCore tagCloudCore)
        {
            TagCloudCore = tagCloudCore ?? new TagCloudCore(ManagerFactory.BlogManagerInstance);
        }

        public ActionResult Index()
        {
            if (TagCloudCore.Tags != null)
            {
                var model = GetModel();
                return View("~/Views/WeBlog/TagCloud.cshtml", model);
            }
            return null;
        }

        protected virtual IEnumerable<TagCloudRenderingModel> GetModel()
        {
            return TagCloudCore.Tags.Select(tag => new TagCloudRenderingModel
            {
                Name = tag.Key,
                Url = TagCloudCore.GetTagUrl(tag.Key),
                Weight = TagCloudCore.GetTagWeightClass(tag.Value)
            });
        }
    }
}