using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Configuration;
using Sitecore.Data.Managers;
using Sitecore.Security.Accounts;
using System.Security;
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
        /// Gets the current blog settings.
        /// </summary>
        /// <returns></returns>
        public static Sitecore.Modules.Eviblog.Items.Settings GetCurrentBlogSettings()
        {
            Item blogList = Context.Database.GetItem("/sitecore/system/Modules/EviBlog/Blogs");
            Item blogSettingsItem = Context.Item;

            foreach (Item blog in blogList.Children)
            {
                if (blog.Fields["BlogID"].Value == BlogManager.GetCurrentBlogID().ToString())
                {
                    blogSettingsItem = blog;
                }
            }

            return new Sitecore.Modules.Eviblog.Items.Settings(blogSettingsItem);
        }

        /// <summary>
        /// Gets the current blog settings.
        /// </summary>
        /// <param name="currentBlog">The current blog.</param>
        /// <returns></returns>
        public static Sitecore.Modules.Eviblog.Items.Settings GetCurrentBlogSettings(Blog currentBlog)
        {
            Item blogList = Context.Database.GetItem("/sitecore/system/Modules/EviBlog/Blogs");
            Item blogSettingsItem = Context.Item;

            foreach (Item blog in blogList.Children)
            {
                if (blog.Fields["BlogID"].Value == currentBlog.ID.ToString())
                {
                    blogSettingsItem = blog;
                }
            }

            return new Sitecore.Modules.Eviblog.Items.Settings(blogSettingsItem);
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
            Item blogList = Context.Database.GetItem("/sitecore/system/Modules/EviBlog/Blogs");

            List<Blog> BlogList = new List<Blog>();

            foreach (Item item in blogList.Children)
            {
                Sitecore.Modules.Eviblog.Items.Settings blogSettings = new Sitecore.Modules.Eviblog.Items.Settings(item);

                Item blog = ItemManager.GetItem(blogSettings.BlogID, Context.Language, Sitecore.Data.Version.Latest, Context.Database);

                Account account = Account.FromName(username, AccountType.User);

                if(blog != null)
                    if(blog.Security.CanWrite(account))
                        BlogList.Add(new Blog(blog));
            }

            return BlogList;
        }
        
        /// <summary>
        /// Gets all blogs.
        /// </summary>
        /// <returns></returns>
        public static List<Blog> GetAllBlogs()
        {
            Item blogList = Context.Database.GetItem("/sitecore/system/Modules/EviBlog/Blogs");
            
            List<Blog> BlogList = new List<Blog>();
            
            foreach (Item item in blogList.Children)
            {
                Sitecore.Modules.Eviblog.Items.Settings blogSettings = new Sitecore.Modules.Eviblog.Items.Settings(item);

                Item blog = ItemManager.GetItem(blogSettings.BlogID, Context.Language, Sitecore.Data.Version.Latest, Context.Database);

                BlogList.Add(new Blog(blog));
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
