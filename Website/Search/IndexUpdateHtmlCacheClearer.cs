using System;
using Sitecore.Data.Managers;
using Sitecore.Events;
using Sitecore.Publishing;

namespace Sitecore.Modules.WeBlog.Search
{
    /// <summary>
    /// Allows clearing of the HTML cache following index update on a CD server.
    /// See http://sitecoreblog.alexshyba.com/2012/07/sync-up-sitecore-search-index-update.html
    /// </summary>
    public class IndexUpdateHtmlCacheClearer : HtmlCacheClearer
    {
        public void OnPropertyChanged(object sender, EventArgs args)
        {
            var propertyName = Event.ExtractParameter(args, 0) as string;
            if (propertyName == null)
            {
                return;
            }

            //only clear cache if the property update is an index update
            if (propertyName.Equals(IndexingManager.LastUpdatePropertyKey))
            {
                base.ClearCache(sender, args);
            }
        }
    }
}