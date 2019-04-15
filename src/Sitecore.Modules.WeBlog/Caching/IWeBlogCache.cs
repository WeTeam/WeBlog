namespace Sitecore.Modules.WeBlog.Caching
{
    /// <summary>
    /// Defines a cache used within the module.
    /// </summary>
    public interface IWeBlogCache
    {
        /// <summary>
        /// Clear all entries from the cache.
        /// </summary>
        void ClearCache();
    }
}