using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Managers
{
    public interface IBlogManager
    {
        /// <summary>
        /// Gets the current blog for the context item
        /// </summary>
        /// <returns>The current blog if found, otherwise null</returns>
        BlogHomeItem GetCurrentBlog();

        /// <summary>
        /// Gets the current blog for the item
        /// </summary>
        /// <param name="item">The item to find the current blog for</param>
        /// <returns>The current blog if found, otherwise null</returns>
        BlogHomeItem GetCurrentBlog(Item item);

        /// Get all blogs the user has write access to
        /// </summary>
        /// <param name="username">The name of the user requiring write access to the blog</param>
        /// <returns>The blogs the user has write access to</returns>
        BlogHomeItem[] GetUserBlogs(string username);        

        /// <summary>
        /// Gets all the blogs.
        /// </summary>
        /// <param name="database">The database to get the blogs from. If null, use the context database</param>
        /// <returns>The list of all blogs</returns>
        BlogHomeItem[] GetAllBlogs(Database database);

        /// <summary>
        /// Checks if the current blog has RSS enabled
        /// </summary>
        /// <returns>True if RSS is enabled, otherwise False</returns>
        bool EnableRSS();

        /// <summary>
        /// Checks if the current blog has RSS enabled
        /// </summary>
        /// <param name="blog">The blog to read the setting from</param>
        /// <returns>True if RSS is enabled, otherwise False</returns>
        bool EnableRSS(BlogHomeItem blog);

        /// <summary>
        /// Checks if emails should be displayed with comments
        /// </summary>
        /// <returns>True if email should be shown, otherwise False</returns>
        bool ShowEmailWithinComments();

        /// <summary>
        /// Checks if emails should be displayed with comments
        /// </summary>
        /// /// <param name="blog">The blog to read the setting from</param>
        /// <returns>True if email should be shown, otherwise False</returns>
        bool ShowEmailWithinComments(BlogHomeItem blog);

        /// <summary>
        /// Returns the dictionary item.
        /// </summary>
        /// <returns>Returns standard dictionary item if there is no custom item selected</returns>
        Item GetDictionaryItem();
    }
}
