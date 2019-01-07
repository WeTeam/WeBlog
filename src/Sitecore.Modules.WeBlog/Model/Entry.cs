using System;
using System.Collections.Generic;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Model
{
    public class Entry
    {
        /// <summary>
        /// The ID of the entry.
        /// </summary>
        public ID ItemId { get; set; }

        /// <summary>
        /// The title of the entry.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The tags of the entry.
        /// </summary>
        public virtual IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// The entry date of the entry.
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// The author of the entry.
        /// </summary>
        public string Author { get; set; }
    }
}