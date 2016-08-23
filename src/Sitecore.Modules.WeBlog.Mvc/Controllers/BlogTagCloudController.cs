using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Components.Parameters;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Mvc.Components.Parameters;
using Sitecore.Modules.WeBlog.Mvc.Model;

namespace Sitecore.Modules.WeBlog.Mvc.Controllers
{
    public class BlogTagCloudController : BlogBaseController
    {
        protected ITagCloudCore TagCloudCore { get; set; }

        public BlogTagCloudController() : this(null) { }

        public int MaximumCount { get; set; }

        public string SortingOptions { get; set; }

        public BlogTagCloudController(ITagCloudCore tagCloudCore)
        {
            TagCloudCore = tagCloudCore ?? new TagCloudCore(ManagerFactory.BlogManagerInstance);
            new RenderingParameterHelper<Controller>(this, true);
        }

        public ActionResult Index()
        {
            if (TagCloudCore.Tags.Any() && MaximumCount > 0)
            {
                var model = new TagCloudRenderingModel
                {
                    SortNames = TagCloudCore.GetSortNames(SortingOptions),
                    Tags = GetTagRenderingModels()
                };
                return View("~/Views/WeBlog/TagCloud.cshtml", model);
            }
            return null;
        }

        protected virtual IEnumerable<TagRenderingModel> GetTagRenderingModels()
        {
            return TagCloudCore.Tags.Take(MaximumCount).Select(tag => new TagRenderingModel(tag)
            {
                Url = TagCloudCore.GetTagUrl(tag.Name),
                Weight = TagCloudCore.GetTagWeightClass(tag.Count)
            });
        }
    }
}