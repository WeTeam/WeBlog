using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Pipelines.GetSummary;
using Sitecore.Pipelines;
using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Layouts.WeBlog
{
    public partial class BlogPostListEntry : System.Web.UI.UserControl
    {
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
    }
}