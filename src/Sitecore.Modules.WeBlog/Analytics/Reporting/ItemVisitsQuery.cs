using System;
using System.Collections.Generic;
using Sitecore.Analytics.Reporting;
using Sitecore.Configuration;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Analytics.Reporting
{
    /// <summary>
    /// A reporting query to retrieve the total visits for an item.
    /// </summary>
    public class ItemVisitsQuery
#if !SC75
        : ItemBasedReportingQuery
#endif
    {
#if SC75
        /// <summary>The <see cref="ReportDataProviderBase"/> to read reporting data from.</summary>
        private ReportDataProviderBase _reportProvider = null;
#endif
    
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="reportProvider">The <see cref="ReportDataProviderBase"/> to read reporting data from.</param>
        public ItemVisitsQuery(ReportDataProviderBase reportProvider = null)
#if !SC75
            : base(Constants.ReportingQueries.ItemVisits, reportProvider)
        {
        }
#else
        {
            _reportProvider = reportProvider ?? (ReportDataProviderBase)Factory.CreateObject("reporting/dataProvider", true);
        }
#endif

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
#if !SC75
        public override void Execute()
#else
        public void Execute()
#endif
        {
            var parameters = new Dictionary<string, object>
            {
                {"@ItemId", ItemId}
            };

#if !SC75
            var dt = this.ExecuteQuery(parameters);
#else
            var query = new ReportDataQuery(Constants.ReportingQueries.ItemVisits.ToString(), parameters);
            var queryResponse = _reportProvider.GetData("item", query, new CachingPolicy());
            var dt = queryResponse.GetDataTable();
#endif
            if (dt != null && dt.Rows.Count > 0)
            {
                var result = dt.Rows[0]["Visits"];
                if(result != null && result != DBNull.Value)
                    Visits = (long)dt.Rows[0]["Visits"];
            }
        }
    }
}