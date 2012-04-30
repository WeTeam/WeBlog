using System;
using System.Drawing;
using System.Web.UI;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogEntry : BaseEntrySublayout
    {
        protected const string CHECKBOX_TRUE = "1";

        public string ShowEntryTitle { get; set; }
        public string ShowEntryImage { get; set; }
        public string ShowEntryMetadata { get; set; }
        public string ShowEntryIntroduction { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //will map rendering properties
            SublayoutParamHelper paramHelper = new SublayoutParamHelper(this, true);

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