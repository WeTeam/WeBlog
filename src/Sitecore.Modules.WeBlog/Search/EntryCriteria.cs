using System;

namespace Sitecore.Modules.WeBlog.Search
{
    /// <summary>
    /// Defines criteria for retriving entries.
    /// </summary>
    public class EntryCriteria
    {
        /// <summary>
        /// Creates criteria which retrieves all entries.
        /// </summary>
        public static EntryCriteria AllEntries =>
            new EntryCriteria
            {
                PageNumber = 1,
                PageSize = int.MaxValue
            };

        /// <summary>
        /// Gets or sets the tag the entry must include.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the category the entry must include.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the minimum date of the entry.
        /// </summary>
        public DateTime? MinimumDate { get; set; }

        /// <summary>
        /// Gets or sets the maximum date of the entry.
        /// </summary>
        public DateTime? MaximumDate { get; set; }

        /// <summary>
        /// Gets or sets the number of the page to retrieve.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the size of the page to retrieve.
        /// </summary>
        public int PageSize { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is EntryCriteria)
            {
                var other = obj as EntryCriteria;

                if (!PageNumber.Equals(other.PageNumber))
                    return false;

                if (!PageSize.Equals(other.PageSize))
                    return false;

                if (Tag == null && other.Tag != null)
                    return false;

                if (Tag != null)
                {
                    if (other.Tag == null || !Tag.Equals(other.Tag))
                        return false;
                }

                if (Category == null && other.Category != null)
                    return false;

                if (Category != null)
                {
                    if (other.Category == null || !Category.Equals(other.Category))
                        return false;
                }

                if (MinimumDate == null && other.MinimumDate != null)
                    return false;

                if (MinimumDate != null)
                {
                    if (other.MinimumDate == null || !MinimumDate.Equals(other.MinimumDate))
                        return false;
                }

                if (MaximumDate == null && other.MaximumDate != null)
                    return false;

                if (MaximumDate != null)
                {
                    if (other.MaximumDate == null || !MaximumDate.Equals(other.MaximumDate))
                        return false;
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            var sum =
                PageNumber.GetHashCode() +
                PageSize.GetHashCode();

            if (Tag != null)
                sum += Tag.GetHashCode();

            if (Category != null)
                sum += Category.GetHashCode();

            if(MinimumDate != null)
                sum += MinimumDate.GetHashCode();

            if(MaximumDate != null)
                sum += MaximumDate.GetHashCode();

            return sum;
        }
    }
}