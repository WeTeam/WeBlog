using System.Drawing;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;
using Sitecore.Pipelines;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogPostListEntryController : BlogBaseController
    {
        public ActionResult Index(PostListEntryRenderingModel model)
        {
            Size maxEntryImage = CurrentBlog.MaximumThumbnailImageSizeDimension;
            if (maxEntryImage != Size.Empty)
            {
                model.MaxWidth = maxEntryImage.Width;
                model.MaxHeight = maxEntryImage.Height;
            }
            model.Summary = GetSummary(model.EntryItem);
            return PartialView(model.PostTemplate, model);
        }

        protected string GetSummary(EntryItem entry)
        {
            var args = new GetSummaryArgs();
            args.Entry = entry;

            CorePipeline.Run("weblogGetSummary", args, true);

            return args.Summary;
        }
    }
}