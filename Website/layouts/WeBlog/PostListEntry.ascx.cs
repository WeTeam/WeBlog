using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;
using Sitecore.Pipelines;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using System.Drawing;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Layouts.WeBlog
{
    public partial class BlogPostListEntry : BaseEntrySublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //will map rendering properties
            SublayoutParamHelper paramHelper = new SublayoutParamHelper(this, true);

            var maxEntryImage = CurrentBlog.MaximumThumbnailImageSizeDimension;
            if (maxEntryImage != Size.Empty)
            {
                EntryImage.MaxWidth = maxEntryImage.Width;
                EntryImage.MaxHeight = maxEntryImage.Height;
            }

            if (EntryImage.Item != null && string.IsNullOrEmpty(EntryImage.Item[EntryImage.Field]))
                EntryImage.Field = "Image";
        }

        protected string GetSummary(EntryItem entry)
        {
            var args = new GetSummaryArgs();
            args.Entry = entry;

#if PRE_65
            CorePipeline.Run("weblogGetSummary", args);
#else
            CorePipeline.Run("weblogGetSummary", args, true);
#endif

            return args.Summary;
        }

        protected DateTime GetPublishDate(EntryItem entry)
        {
            var publishDate = ((Item)entry).Publishing.PublishDate;
            var createdDate = entry.Created;
            return (publishDate > createdDate) ? publishDate : createdDate;
        }
    }
}