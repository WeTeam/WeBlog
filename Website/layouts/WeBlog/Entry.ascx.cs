using System;
using System.Drawing;
using System.Web.UI;
using System.ComponentModel;
using Sitecore.Security.Authentication;
using Sitecore.Security.Accounts;

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
            if(maxEntryImage != Size.Empty)
            {
                EntryImage.MaxWidth = maxEntryImage.Width;
                EntryImage.MaxHeight = maxEntryImage.Height;
            }
        }

        /// <summary>
        /// Determines if the field given by name needs to have it's output wrapped in an additional tag
        /// </summary>
        /// <param name="fieldName">The name of the field to check</param>
        /// <returns>True if wrapping is required, otherwsie false  </returns>
        protected bool DoesFieldRequireWrapping(string fieldName)
        {
            return !CurrentEntry[fieldName].StartsWith("<p>");
        }
    }
}