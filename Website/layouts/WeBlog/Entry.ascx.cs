using System;
using System.Drawing;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Utilities;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogEntry : BaseEntrySublayout
    {
        protected const string CHECKBOX_TRUE = "1";

        public string ShowEntryTitle { get; set; }
        public string ShowEntryImage { get; set; }
        public string ShowEntryMetadata { get; set; }

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
    }
}