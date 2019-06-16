using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Caching
{
    /// <summary>
    /// A key in the <see cref="EntrySearchCache"/>.
    /// </summary>
    public class EntrySearchCacheKey
    {
        /// <summary>
        /// Gets the <see cref="EntryCriteria"/> of the key.
        /// </summary>
        public EntryCriteria EntryCriteria { get; }

        /// <summary>
        /// Gets the order of the results of the key.
        /// </summary>
        public ListOrder ResultOrder { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="entryCriteria">The <see cref="EntryCriteria"/> used for the search.</param>
        /// <param name="resultOrder">The order of the results.</param>
        public EntrySearchCacheKey(EntryCriteria entryCriteria, ListOrder resultOrder)
        {
            Assert.ArgumentNotNull(entryCriteria, nameof(entryCriteria));

            EntryCriteria = entryCriteria;
            ResultOrder = resultOrder;
        }

        public override bool Equals(object obj)
        {
            if(obj is EntrySearchCacheKey)
            {
                var other = obj as EntrySearchCacheKey;

                return
                    EntryCriteria.Equals(other.EntryCriteria) &&
                    ResultOrder.Equals(other.ResultOrder);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return EntryCriteria.GetHashCode() + ResultOrder.GetHashCode();
        }
    }
}