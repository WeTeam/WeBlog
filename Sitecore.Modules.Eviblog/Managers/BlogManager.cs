using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Configuration;
namespace Sitecore.Modules.Eviblog.Managers
{
    public class BlogManager
    {
        /// <summary>
        /// Gets the current blog ID.
        /// </summary>
        /// <returns></returns>
        public static ID GetCurrentBlogID()
        {
            Item currentItem = Context.Item;

            if (currentItem.TemplateID.ToString() != Settings.Default.BlogTemplateID)
            {
                while (currentItem.TemplateID.ToString() != Settings.Default.BlogTemplateID)
                {
                    currentItem = currentItem.Parent;
                }
            }
            return currentItem.ID;
        }

        /// <summary>
        /// Gets the current blog.
        /// </summary>
        /// <returns></returns>
        public static Blog GetCurrentBlog()
        {
            Item currentItem = Context.Item;

            if (currentItem.TemplateID.ToString() != Settings.Default.BlogTemplateID)
            {
                while (currentItem.TemplateID.ToString() != Settings.Default.BlogTemplateID)
                {
                    currentItem = currentItem.Parent;
                }
            }

            Blog currentBlog = new Blog(currentItem);

            return currentBlog;
        }

        /// <summary>
        /// Gets the current blog item.
        /// </summary>
        /// <returns></returns>
        public static Item GetCurrentBlogItem()
        {
            Item currentItem = Context.Item;

            if (currentItem.TemplateID.ToString() != Settings.Default.BlogTemplateID)
            {
                while (currentItem.TemplateID.ToString() != Settings.Default.BlogTemplateID)
                {
                    currentItem = currentItem.Parent;
                }
            }

            return currentItem;
        }

        /// <summary>
        /// Gets the current blog item by item.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Item GetCurrentBlogItem(ID source, string dbname)
        {
            Database currentDB = Factory.GetDatabase(dbname);
            Item currentItem = currentDB.GetItem(source);

            if (currentItem.TemplateID.ToString() != Settings.Default.BlogTemplateID)
            {
                while (currentItem.TemplateID.ToString() != Settings.Default.BlogTemplateID)
                {
                    currentItem = currentItem.Parent;
                }
            }

            return currentItem;
        }


        /// <summary>
        /// Gets all blogs.
        /// </summary>
        /// <returns></returns>
        public static List<Blog> GetUserBlogs(string username)
        {
            List<Blog> BlogList = new List<Blog>();

            foreach (Item item in Context.Database.GetItem("/sitecore/content" + Context.Site.StartItem).Children)
            {
                if (item.TemplateID.ToString() == Settings.Default.BlogTemplateID && item.Versions.GetVersions().Length > 0 && item.Statistics.UpdatedBy == username)
                {
                    BlogList.Add(new Blog(item));
                }
            }

            return BlogList;
        }
        
        /// <summary>
        /// Gets all blogs.
        /// </summary>
        /// <returns></returns>
        public static List<Blog> GetAllBlogs()
        {
            List<Blog> BlogList = new List<Blog>();
            
            foreach (Item item in Context.Database.GetItem(Context.Site.StartItem).Children)
            {
                if (item.TemplateID.ToString() == Settings.Default.BlogTemplateID && item.Versions.GetVersions().Length > 0)
                {
                    BlogList.Add(new Blog(item));
                }
            }

            return BlogList;
        }


        /// <summary>
        /// Gets the blog by ID.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <returns></returns>
        public static Item GetBlogByID(ID BlogID)
        {
            Item Blog = Context.Database.GetItem(BlogID);
            
            if (Blog != null)
            {
                try
                {
                    return Blog;
                }
                catch(ApplicationException)
                {
                    Diagnostics.Log.Error("Could not find blog item:" + BlogID, Blog);
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Enables the RSS.
        /// </summary>
        /// <returns></returns>
        public static bool EnableRSS()
        {
            Blog current = GetCurrentBlog();

            return current.EnableRSS;
        }

        /// <summary>
        /// Enables the comments.
        /// </summary>
        /// <returns></returns>
        public static bool EnableComments()
        {
            Blog current = GetCurrentBlog();

            return current.EnableComments;
        }

        /// <summary>
        /// Shows the email within comments.
        /// </summary>
        /// <returns></returns>
        public static bool ShowEmailWithinComments()
        {
            Blog current = GetCurrentBlog();
            
            return current.ShowEmailWithinComments;
        }
    }
}
