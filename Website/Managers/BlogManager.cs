using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with blogs
    /// </summary>
    public class BlogManager : IBlogManager
    {
        /// <summary>
        /// Gets the current blog for the context item
        /// </summary>
        /// <returns>The current blog if found, otherwise null</returns>
        public BlogHomeItem GetCurrentBlog()
        {
            return GetCurrentBlog(Context.Item);
        }

        /// <summary>
        /// Gets the current blog for the item
        /// </summary>
        /// <param name="item">The item to find the current blog for</param>
        /// <returns>The current blog if found, otherwise null</returns>
        public BlogHomeItem GetCurrentBlog(Item item)
        {
            var blogItem = item.GetCurrentItem(Settings.BlogTemplateIDString);

            if (blogItem != null)
                return new BlogHomeItem(blogItem);
            else
                return null;
        }

        /// <summary>
        /// Get all blogs the user has write access to from the content database
        /// </summary>
        /// <param name="username">The name of the user requiring write access to the blog</param>
        /// <returns>The blogs the user has write access to</returns>
        public BlogHomeItem[] GetUserBlogs(string username)
        {
            var blogList = new List<BlogHomeItem>();

            var blogs = GetAllBlogs(ContentHelper.GetContentDatabase());
            var account = Account.FromName(username, AccountType.User);

            foreach (var blog in blogs)
            {
                if (blog.InnerItem.Security.CanWrite(account))
                    blogList.Add(blog);
            }

            return blogList.ToArray();
        }
        
        /// <summary>
        /// Gets all the blogs.
        /// </summary>
        /// <param name="database">The database to get the blogs from. If null, use the context database</param>
        /// <returns>The list of all blogs</returns>
        public BlogHomeItem[] GetAllBlogs(Database database)
        {
            if (database == null)
                database = Context.Database;

            // TODO: Store the result of this call in cache and clear it from cache on publish
            var blogTemplate = database.GetTemplate(Settings.BlogTemplateIDString);
            var contentRoot = database.GetItem(Settings.ContentRootPath);
            var blogItems = contentRoot.FindItemsByTemplateOrDerivedTemplate(blogTemplate);
            return (from item in blogItems select new BlogHomeItem(item)).ToArray();
        }

        /// <summary>
        /// Checks if the current blog has RSS enabled
        /// </summary>
        /// <returns>True if RSS is enabled, otherwise False</returns>
        public bool EnableRSS()
        {
            var current = GetCurrentBlog();
            return EnableRSS(current);
        }

        /// <summary>
        /// Checks if the current blog has RSS enabled
        /// </summary>
        /// <param name="blog">The blog to read the setting from</param>
        /// <returns>True if RSS is enabled, otherwise False</returns>
        public bool EnableRSS(BlogHomeItem blog)
        {
            return blog.EnableRss.Checked;
        }

        /// <summary>
        /// Checks if emails should be displayed with comments
        /// </summary>
        /// <returns>True if email should be shown, otherwise False</returns>
        public bool ShowEmailWithinComments()
        {
            var current = GetCurrentBlog();
            return ShowEmailWithinComments(current);
        }

        /// <summary>
        /// Checks if emails should be displayed with comments
        /// </summary>
        /// /// <param name="blog">The blog to read the setting from</param>
        /// <returns>True if email should be shown, otherwise False</returns>
        public bool ShowEmailWithinComments(BlogHomeItem blog)
        {
            return blog.ShowEmailWithinComments.Checked;
        }

        /// <summary>
        /// Returns the dictionary item.
        /// </summary>
        /// <returns>Returns standard dictionary item if there is no custom item selected</returns>
        public Item GetDictionaryItem()
        {
            BlogHomeItem currentBlog = GetCurrentBlog();
            if (currentBlog != null)
                return currentBlog.DictionaryItem;
            else
                return null;
        }
    }
}
