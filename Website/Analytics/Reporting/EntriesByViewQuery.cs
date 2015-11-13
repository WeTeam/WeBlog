using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Sitecore.Analytics.Reporting;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Analytics.Reporting
{
  /// <summary>
  /// A reporting query to order Item IDs by views
  /// </summary>
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
    {
      var entryIdStr = string.Join("','", this.EntryIds);

      var dt = this.ExecuteQuery(new Dictionary<string, object>
      {
        { "@EntryIds", entryIdStr }
      });

      if (dt != null)
      {
        OrderedEntryIds = from DataRow row in dt.Rows
          select new ID((Guid) row["ItemId"]);
      }
    }
#else
    public void Execute()
    {
      var entryIdStr = string.Join("','", this.EntryIds);

      var query = new ReportDataQuery(Constants.ReportingQueries.EntriesByView.ToString(), new Dictionary<string, object>
      {
        {"@EntryIds", entryIdStr}
      });
      
      /*var dt = this.ExecuteQuery(new Dictionary<string, object>
      {
        { "@EntryIds", entryIdStr }
      });
       * */

      var queryResponse = _reportProvider.GetData("item", query, new CachingPolicy());
      var dt = queryResponse.GetDataTable();

      if (dt != null)
      {
        OrderedEntryIds = from DataRow row in dt.Rows
                          select new ID((Guid)row["ItemId"]);
      }
    }
#endif
  }
}