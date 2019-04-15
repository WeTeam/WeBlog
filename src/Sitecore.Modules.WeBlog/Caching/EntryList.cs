using Sitecore.Caching;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Reflection;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Caching
{
    /// <summary>
    /// A list of entries.
    /// </summary>
    public class EntryList : ICacheable
    {
        private long _size = 0;

        private List<Entry> _entries = null;

        /// <summary>
        /// Indicates whether the object can be cached.
        /// </summary>
        public bool Cacheable { get; set; }        

        /// <summary>
        /// Indicates whether the object can be changed once cached.
        /// </summary>
        public bool Immutable => true;

        /// <summary>
        /// An event used to indicate the data length has changed.
        /// </summary>
        public event DataLengthChangedDelegate DataLengthChanged;

        /// <summary>
        /// Gets or sets the entries to store in the list.
        /// </summary>
        public List<Entry> Entries
        {
            get
            {
                return _entries;
            }

            set
            {
                _entries = value;
                _size = CalculateSize(value);
                DataLengthChanged?.Invoke(this);
            }
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public EntryList()
        {
            Cacheable = true;
            Entries = new List<Entry>();
        }

        /// <summary>
        /// Gets the size of the object.
        /// </summary>
        /// <returns></returns>
        public long GetDataLength()
        {
            return _size;
        }

        /// <summary>
        /// Calculates the size of the list of entries.
        /// </summary>
        /// <param name="entries">The entries to calculate the size of.</param>
        /// <returns>The size of the entries.</returns>
        private long CalculateSize(List<Entry> entries)
        {
            var size = 0L;

            foreach(var entry in entries)
            {
                size +=
                    TypeUtil.SizeOfString(entry.Title) + // Title
                    8 + // EntryDate
                    TypeUtil.SizeOfString(entry.Author); // Author

                if(entry.Uri != null)
                {
                    size +=
                        TypeUtil.SizeOfID() + // ItemUri.ID
                        TypeUtil.SizeOfLanguage() + // ItemUri.Language
                        1 + // ItemUri.IsEmpty
                        TypeUtil.SizeOfString(entry.Uri.DatabaseName) + // ItemUri.DatabaseName
                        TypeUtil.SizeOfVersion() + // ItemUri.Version
                        TypeUtil.SizeOfString(entry.Uri.Path); // ItemUri.Path
                }

                if (entry.Tags != null)
                {
                    foreach (var tag in entry.Tags)
                    {
                        size += TypeUtil.SizeOfString(tag);
                    }
                }
            }

            return size;
        }
    }
}