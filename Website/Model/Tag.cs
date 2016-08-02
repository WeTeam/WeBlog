using System;

namespace Sitecore.Modules.WeBlog.Model
{
    /// <summary>
    /// Represents a tag applied to one or more blog entries.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// The name of the tag itself.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// The date of the most recent entry in which the tag was used.
        /// </summary>
        public DateTime LastUsed
        {
            get;
            set;
        }

        /// <summary>
        /// The number of times the tag has been applied to an entry or entries.
        /// </summary>
        public int Count
        {
            get;
            set;
        }
    }
}