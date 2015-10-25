using System.ComponentModel;
using System.Drawing;
using System.Web.Mvc;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Controllers
{
    public class BlogEntryController : BlogBaseController
    {
        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryTitle { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryImage { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryMetadata { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryIntroduction { get; set; }

        public ActionResult Index()
        {
            var model = new EntryRenderingModel
            {
                Title = CurrentEntry.Title.Text + " | " + CurrentBlog.Title.Text,
                CurrentEntry = CurrentEntry,
                ShowEntryTitle = ShowEntryTitle,
                ShowEntryImage = ShowEntryImage,
                ShowEntryMetadata = ShowEntryMetadata,
                ShowEntryIntroduction = ShowEntryIntroduction
            };

            var maxEntryImage = CurrentBlog.MaximumEntryImageSizeDimension;
            if (maxEntryImage != Size.Empty)
            {
                model.MaxWidth = maxEntryImage.Width;
                model.MaxHeight = maxEntryImage.Height;
            }

            return View("~/Views/WeBlog/Entry.cshtml", model);
        }
    }
}