using System;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Utilities
{
    public static class DataUtil
    {
        /// <summary>
        /// Gets the content database
        /// </summary>
        /// <returns>The content database</returns>
        public static Database GetContentDatabase()
        {
            var site = Sitecore.Configuration.Factory.GetSite(Sitecore.Constants.ShellSiteName);
            return site.ContentDatabase;
        }
    }
}