using System.ComponentModel;
using System.Drawing;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class Entry : BlogRenderingModelBase
    {
        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryTitle { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryImage { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryMetadata { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryIntroduction { get; set; }

        public string Title { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);

            Title = CurrentEntry.DisplayName;

            if (string.IsNullOrEmpty(Title))
                Title = CurrentEntry.Title.Text;
            
            // todo: Move this to a pipeline
            Title += " | " + CurrentBlog.Title.Text;

            var maxEntryImage = CurrentBlog.MaximumEntryImageSizeDimension;
            if (maxEntryImage != Size.Empty)
            {
                MaxWidth = maxEntryImage.Width;
                MaxHeight = maxEntryImage.Height;
            }
        }
    }
}