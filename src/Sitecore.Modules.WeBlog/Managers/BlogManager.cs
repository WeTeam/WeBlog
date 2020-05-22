using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Security.Accounts;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Abstractions;
using System;

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with blogs
    /// </summary>
    public class BlogManager : IBlogManager
    {
        /// <summary>
        /// The <see cref="IWeBlogSettings"/> to access settings from.
        /// </summary>
        protected IWeBlogSettings Settings = null;

        /// <summary>
        /// The <see cref="BaseLinkManager"/> used to generate links.
        /// </summary>
        protected BaseLinkManager LinkManager { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="settings">The settings to use. If null, the default settings are used.</param>
        public BlogManager(BaseLinkManager linkManager, IWeBlogSettings settings = null)
        {
            if (linkManager == null)
                throw new ArgumentNullException(nameof(linkManager));

            LinkManager = linkManager;
            Settings = settings ?? WeBlogSettings.Instance;
        }

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
            var blogItem = item.FindAncestorByAnyTemplate(Settings.BlogTemplateIds);

            if (blogItem != null)
                return new BlogHomeItem(blogItem, LinkManager);
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
        public BlogHomeItem[] GetAllBlogs(Database database = null)
        {
            if (database == null)
                database = Context.Database;

            var contentRoot = database.GetItem(Settings.ContentRootPath);

            return (from templateId in Settings.BlogTemplateIds
                   let template = database.GetTemplate(templateId)
                   let blogItems = contentRoot.FindItemsByTemplateOrDerivedTemplate(template)
                   from item in blogItems
                   where item != null
                    select new
                    BlogHomeItem(item, LinkManager)
                    ).ToArray();
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
            if (blog == null)
                return false;

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
            if (blog == null)
                return false;

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
