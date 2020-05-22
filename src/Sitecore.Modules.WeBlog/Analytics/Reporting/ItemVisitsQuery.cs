using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Xdb.Reporting;

namespace Sitecore.Modules.WeBlog.Analytics.Reporting
{
    /// <summary>
    /// A reporting query to retrieve the total visits for an item.
    /// </summary>
    public class ItemVisitsQuery
        : ItemBasedReportingQuery
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="reportProvider">The <see cref="ReportDataProviderBase"/> to read reporting data from.</param>
        public ItemVisitsQuery(ReportDataProviderBase reportProvider = null)
            : base(Constants.ReportingQueries.ItemVisits, reportProvider)
        {
        }

        /// <summary>
        /// Gets or sets the ID of the item to retrieve the visits for.
        /// </summary>
        public ID ItemId { get; set; }

        /// <summary>
        /// Gets the visit count.
        /// </summary>
        public long Visits { get; protected set; }

        /// <summary>
        /// Executes the query against the reporting database.
        /// </summary>
        public override void Execute()
        {
            var parameters = new Dictionary<string, object>
            {
                {"@ItemId", ItemId}
            };

            var dt = this.ExecuteQuery(parameters);
            
            if (dt != null && dt.Rows.Count > 0)
            {
                var result = dt.Rows[0]["Visits"];
                if(result != null && result != DBNull.Value)
                    Visits = (long)dt.Rows[0]["Visits"];
            }
        }
    }
}