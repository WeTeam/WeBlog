using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.Eviblog.Comparers;
using Sitecore.Modules.Eviblog.Items;

namespace Sitecore.Modules.Eviblog.Managers
{
    public class EntryManager
    {
        /// <summary>
        /// Gets the current blog entry.
        /// </summary>
        /// <returns></returns>
        public static Entry GetCurrentBlogEntry()
        {
            Entry current = new Entry(Context.Item);

            return current;
        }

        public static bool DeletePost(string postID)
        {
            Item blogPost = EntryManager.GetBlogEntryByID(new ID(postID));

            try
            {
                blogPost.Delete();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets all blogs.
        /// </summary>
        /// <returns></returns>
        public static List<Entry> GetAllEntries()
        {
            List<Entry> EntryList = new List<Entry>();

            foreach (Item item in Context.Database.GetItem(Context.Site.StartPath).Children)
            {
                if (item.TemplateID.ToString() == Settings.Default.BlogTemplateID &&
                    item.Versions.GetVersions().Length > 0)
                {
                    foreach (Entry entry in GetBlogEntries(item.ID))
                    {
                        EntryList.Add(entry);
                    }
                }
            }

            return EntryList;
        }

        /// <summary>
        /// Gets all entries.
        /// </summary>
        /// <param name="MaxNumber">The max number of entry items.</param>
        /// <returns></returns>
        public static List<Entry> GetAllEntries(int MaxNumber)
        {
            List<Entry> EntryList = new List<Entry>();
            int count = 0;

            foreach (Item item in Context.Database.GetItem(Context.Site.StartPath).Children)
            {
                if (item.TemplateID.ToString() == Settings.Default.BlogTemplateID &&
                    item.Versions.GetVersions().Length > 0)
                {
                    foreach (Entry entry in GetBlogEntries(item.ID))
                    {
                        if (count < MaxNumber)
                        {
                            EntryList.Add(entry);
                            count++;
                        }
                    }
                }
            }

            return EntryList;
        }

        /// <summary>
        /// Gets all blog posts.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <returns></returns>
        public static List<Entry> GetBlogEntries(ID BlogID)
        {
            List<Entry> BlogPostList = new List<Entry>();

            Item Blog = Context.Database.GetItem(BlogID);

            foreach (Entry entry in MakeSortedEntriesList(Blog.Children.ToArray()))
            {
                BlogPostList.Add(entry);
            }

            return BlogPostList;
        }

        /// <summary>
        /// Gets all blog posts.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <param name="MaxNumber">The max number.</param>
        /// <returns></returns>
        public static List<Entry> GetBlogEntries(ID BlogID, int MaxNumber)
        {
            List<Entry> BlogPostList = new List<Entry>();
            int count = 0;

            Item Blog = Context.Database.GetItem(BlogID);

            foreach (Entry entry in MakeSortedEntriesList(Blog.Children.ToArray()))
            {
                if (count < MaxNumber)
                {
                    BlogPostList.Add(entry);
                    count++;
                }
            }

            return BlogPostList;
        }

        /// <summary>
        /// Gets the blog entries.
        /// </summary>
        /// <param name="Tag">The tag.</param>
        /// <returns></returns>
        public static List<Entry> GetBlogEntries(string Tag)
        {
            List<Entry> BlogPostList = new List<Entry>();
            Item Blog = Context.Database.GetItem(BlogManager.GetCurrentBlogID());

            foreach (Entry entry in MakeSortedEntriesList(Blog.Children.ToArray()))
            {
                if (entry.Tags.Contains(Tag))
                {
                    BlogPostList.Add(entry);
                }
            }

            return BlogPostList;
        }

        /// <summary>
        /// Gets the blog entries by month and year.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public static List<Entry> GetBlogEntriesByMonthAndYear(int month, int year)
        {
            List<Entry> BlogPostList = new List<Entry>();
            Item Blog = Context.Database.GetItem(BlogManager.GetCurrentBlogID());

            foreach (Entry entry in MakeSortedEntriesList(Blog.Children.ToArray()))
            {
                if (entry.Created.Month == month && entry.Created.Year == year )
                {
                    BlogPostList.Add(entry);
                }
            }

            return BlogPostList;
        }

        /// <summary>
        /// Gets the blog entries.
        /// </summary>
        /// <param name="Tag">The tag.</param>
        /// <param name="MaxNumber">The max number.</param>
        /// <returns></returns>
        public static List<Entry> GetBlogEntries(string Tag, int MaxNumber)
        {
            List<Entry> BlogPostList = new List<Entry>();
            int count = 0;

            Item Blog = Context.Database.GetItem(BlogManager.GetCurrentBlogID());

            foreach (Entry entry in MakeSortedEntriesList(Blog.Children.ToArray()))
            {
                if (count < MaxNumber && entry.Tags.Contains(Tag))
                {
                    BlogPostList.Add(entry);
                    count++;
                }
            }

            return BlogPostList;
        }

        /// <summary>
        /// Gets the blog posty by ID.
        /// </summary>
        /// <param name="BlogPostID">The blog post ID.</param>
        /// <returns></returns>
        public static Item GetBlogEntryByID(ID BlogPostID)
        {
            Item Blog = Context.Database.GetItem(BlogPostID);

            if (Blog != null)
            {
                try
                {
                    return Blog;
                }
                catch (ApplicationException)
                {
                    Log.Error("Could not find blog item:" + BlogPostID, Blog);
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the blog posty by ID.
        /// </summary>
        /// <param name="BlogPostID">The blog post ID.</param>
        /// <returns></returns>
        public static Entry GetBlogEntry(ID BlogPostID)
        {
            Item entry = Context.Database.GetItem(BlogPostID);

            if (entry != null)
            {
                try
                {
                    return new Entry(entry);
                }
                catch (ApplicationException)
                {
                    
                    return null;
                }
            }

            return null;
        }


        /// <summary>
        /// Gets the blog entry by categorie.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <param name="CategorieName">Name of the categorie.</param>
        /// <returns></returns>
        public static List<Entry> GetBlogEntryByCategorie(ID BlogID, string CategorieName)
        {
            List<Entry> BlogPostList = new List<Entry>();

            Item Blog = BlogManager.GetBlogByID(BlogID);

            foreach (Entry entry in MakeSortedEntriesList(Blog.Children.ToArray()))
            {
                if (entry.CategoriesText.Contains(CategorieName))
                {
                    BlogPostList.Add(entry);
                }
            }

            return BlogPostList;
        }

        /// <summary>
        /// Makes the sorted post item list.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static List<Entry> MakeSortedEntriesList(IList array)
        {
            List<Entry> postItemList = new List<Entry>();
            foreach (Item item in array)
            {
                if (item.TemplateID.ToString() == Settings.Default.EntryTemplateID &&
                    item.Versions.GetVersions().Length > 0)
                {
                    postItemList.Add(new Entry(item));
                }
            }
            postItemList.Sort(new PostDateComparerDesc());
            return postItemList;
        }
    }
}