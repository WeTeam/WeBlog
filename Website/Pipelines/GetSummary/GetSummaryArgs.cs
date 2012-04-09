using System;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Pipelines;

namespace Sitecore.Modules.WeBlog.Pipelines.GetSummary
{
    public class GetSummaryArgs : PipelineArgs
    {
        /// <summary>
        /// Gets or sets the entry to get the summary for
        /// </summary>
        public EntryItem Entry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the summary
        /// </summary>
        public string Summary
        {
            get;
            set;
        }
    }
}