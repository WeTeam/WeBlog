using System;
using System.Drawing;
using System.ComponentModel;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogEntry : BaseEntrySublayout
    {
        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryTitle { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryImage
        {
            get { return EntryImage.Visible; }
            set { EntryImage.Visible = value; }
        }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryMetadata { get; set; }

        [TypeConverter(typeof(Converters.ExtendedBooleanConverter))]
        public bool ShowEntryIntroduction { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = CurrentEntry.Title.Text + " | " + CurrentBlog.Title.Text;

            var maxEntryImage = CurrentBlog.MaximumEntryImageSizeDimension;
            if (maxEntryImage != Size.Empty)
            {
                EntryImage.MaxWidth = maxEntryImage.Width;
                EntryImage.MaxHeight = maxEntryImage.Height;
            }
        }

        protected bool DoesFieldRequireWrapping(string fieldName)
        {
            return CurrentEntry.DoesFieldRequireWrapping(fieldName);
        }
    }
}