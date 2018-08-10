using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sitecore.Data;

#if SC90
using Sitecore.Xdb.Reporting;
#else
using Sitecore.Analytics.Reporting;
#endif

namespace Sitecore.Modules.WeBlog.Analytics.Reporting
{
    /// <summary>
    /// A reporting query to order Item IDs by views
    /// </summary>
    [Obsolete("Use the ItemVisitsQuery class instead.")] // Deprecated in release 2.5
    public class EntriesByViewQuery
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
        public EntriesByViewQuery(ReportDataProviderBase reportProvider = null)
#if !SC75
            : base(Constants.ReportingQueries.EntriesByView, reportProvider)
        {
        }
#else
        {
            _reportProvider = reportProvider ?? new ReportDataProvider();
        }
#endif

        /// <summary>
        /// Gets or sets the IDs of the entries to get the visits for.
        /// </summary>
        public IEnumerable<ID> EntryIds { get; set; }

        /// <summary>
        /// Gets the entry IDs ordered by visits.
        /// </summary>
        public IEnumerable<ID> OrderedEntryIds { get; protected set; }

        /// <summary>
        /// Executes the query against the reporting database.
        /// </summary>
#if !SC75
        public override void Execute()
#else
        public void Execute()
#endif
        {
            var entryIdStr = string.Join("','", this.EntryIds);
            var parameters = new Dictionary<string, object>
            {
                {"@EntryIds", entryIdStr}
            };
#if !SC75

            var dt = this.ExecuteQuery(parameters);
#else
            var query = new ReportDataQuery(Constants.ReportingQueries.EntriesByView.ToString(), parameters);
            var queryResponse = _reportProvider.GetData("item", query, new CachingPolicy());
            var dt = queryResponse.GetDataTable();
#endif
            if (dt != null)
            {
                OrderedEntryIds = from DataRow row in dt.Rows
                                  select new ID((Guid)row["ItemId"]);
            }
        }

    }
}