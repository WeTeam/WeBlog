using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Items.Blog;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with blogs
    /// </summary>
    public static class BlogManager
    {
        /// <summary>
        /// Gets the current blog for the context item
        /// </summary>
        /// <returns>The current blog if found, otherwise null</returns>
        public static Items.Blog.BlogItem GetCurrentBlog()
        {
            return GetCurrentBlog(Context.Item);
        }

        /// <summary>
        /// Gets the current blog for the item
        /// </summary>
        /// <param name="item">The item to find the current blog for</param>
        /// <returns>The current blog if found, otherwise null</returns>
        public static Items.Blog.BlogItem GetCurrentBlog(Item item)
        {
            var blogItem = Utilities.Items.GetCurrentItem(item, Settings.BlogTemplateIdString);

            if (blogItem != null)
                return new Items.Blog.BlogItem(blogItem);
            else
                return null;
        }

        /// Get all blogs the user has write access to
        /// </summary>
        /// <param name="username">The name of the user requiring write access to the blog</param>
        /// <returns>The blogs the user has write access to</returns>
        public static Items.Blog.BlogItem[] GetUserBlogs(string username)
        {
            var blogList = new List<Items.Blog.BlogItem>();

            var blogs = GetAllBlogs();
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
        /// <returns>The list of all blogs</returns>
        public static Items.Blog.BlogItem[] GetAllBlogs()
        {
            // TODO: Store the result of this call in cache and clear it from cache on publish
            var blogTemplate = Context.Database.GetTemplate(Settings.BlogTemplateIdString);
            var contentRoot = Context.Database.GetItem(Settings.ContentRootPath);
            var blogItems = Utilities.Items.FindItemsByTemplateOrDerivedTemplate(contentRoot, blogTemplate);
            return (from item in blogItems select new BlogItem(item)).ToArray();
        }

        /// <summary>
        /// Checks if the current blog has RSS enabled
        /// </summary>
        /// <returns>True if RSS is enabled, otherwise False</returns>
        public static bool EnableRSS()
        {
            var current = GetCurrentBlog();
            return EnableRSS(current);
        }

        /// <summary>
        /// Checks if the current blog has RSS enabled
        /// </summary>
        /// <param name="blog">The blog to read the setting from</param>
        /// <returns>True if RSS is enabled, otherwise False</returns>
        public static bool EnableRSS(Items.Blog.BlogItem blog)
        {
            return blog.EnableRSS.Checked;
        }

        /// <summary>
        /// Checks if emails should be displayed with comments
        /// </summary>
        /// <returns>True if email should be shown, otherwise False</returns>
        public static bool ShowEmailWithinComments()
        {
            var current = GetCurrentBlog();
            return ShowEmailWithinComments(current);
        }

        /// <summary>
        /// Checks if emails should be displayed with comments
        /// </summary>
        /// /// <param name="blog">The blog to read the setting from</param>
        /// <returns>True if email should be shown, otherwise False</returns>
        public static bool ShowEmailWithinComments(Items.Blog.BlogItem blog)
        {
            return blog.ShowEmailWithinComments.Checked;
        }

        #region Obsolete Methods
        /// <summary>
        /// Gets the current blog ID.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use GetCurrentBlog().ID instead")]
        public static ID GetCurrentBlogID()
        {
            return GetCurrentBlog().ID;
        }

        /// <summary>
        /// Gets the current blog item.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use GetCurrentBlog().InnerItem instead")]
        public static Item GetCurrentBlogItem()
        {
            return GetCurrentBlog().InnerItem;
        }

        /// <summary>
        /// Gets the current blog item by item.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        [Obsolete("Use GetCurrentBlog(Item item).InnerItem instead")]
        public static Item GetCurrentBlogItem(ID source, string dbname)
        {
            var database = Factory.GetDatabase(dbname);
            Assert.IsNotNull(database, "Failed to find database");

            var item = database.GetItem(source);
            Assert.IsNotNull(item, "Failed to find item");

            return GetCurrentBlog(item).InnerItem;
        }

        /// <summary>
        /// Gets the blog by ID.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <returns></returns>
        [Obsolete("Not required. Just get the item from the database")]
        public static Item GetBlogByID(ID BlogID)
        {
            Item Blog = Context.Database.GetItem(BlogID);

            if (Blog != null)
            {
                try
                {
                    return Blog;
                }
                catch (ApplicationException)
                {
                    Diagnostics.Log.Error("Could not find blog item:" + BlogID, Blog);
                    return null;
                }
            }

            return null;
        }
        #endregion
    }
}
