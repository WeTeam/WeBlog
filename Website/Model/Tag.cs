using System;

namespace Sitecore.Modules.WeBlog.Model
{
    /// <summary>
    /// Represents a tag applied to one or more blog entries.
    /// </summary>
    public class Tag : IEquatable<Tag>
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

        public Tag()
        {
        }

        public Tag(string name, DateTime lastUsed, int count)
        {
            Name = name;
            LastUsed = lastUsed;
            Count = count;
        }

        public bool Equals(Tag other)
        {
            if (other == null)
                return false;

            if (Name.Equals(other.Name) &&
                LastUsed.Equals(other.LastUsed) &&
                Count.Equals(other.Count))
                return true;

            return false;
        }
    }
}