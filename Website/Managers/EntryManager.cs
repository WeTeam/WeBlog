using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Comparers;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using System.Threading;
using Sitecore.StringExtensions;
using Sitecore.Search;
using Sitecore.Analytics;
#if !PRE_65
using Sitecore.Analytics.Data.DataAccess.DataAdapters;
#endif

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with blog entries
    /// </summary>
    public class EntryManager : IEntryManager
    {
        /// <summary>
        /// Gets the current context item as a blog entry
        /// </summary>
        /// <returns>The current blog entry</returns>
        public EntryItem GetCurrentBlogEntry()
        {
            var current = new EntryItem(Context.Item);
            return current;
        }

        /// <summary>
        /// Gets the current context item as a blog entry
        /// </summary>
        /// <param name="item">The item to find the current entry item for</param>
        /// <returns>The current blog entry</returns>
        public EntryItem GetCurrentBlogEntry(Item item)
        {
            var entryItem = Utilities.Items.GetCurrentItem(item, Settings.EntryTemplateIdString);

            if (entryItem != null)
                return new Items.WeBlog.EntryItem(entryItem);
            else
                return null;
        }

        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="postID">The ID of the post to delete</param>
        /// <returns>True if the post was deleted, otherwise False</returns>
        public bool DeleteEntry(string postID)
        {
            return DeleteEntry(postID, Sitecore.Context.Database);
        }

        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="postID">The ID of the post to delete</param>
        /// <param name="db">The database to delete the entry from</param>
        /// <returns>True if the post was deleted, otherwise False</returns>
        public bool DeleteEntry(string postID, Database db)
        {
            Assert.IsNotNull(db, "Database cannot be null");

            if (!string.IsNullOrEmpty(postID))
            {
                var blogPost = db.GetItem(postID);

                if (blogPost != null)
                {
                    try
                    {
                        blogPost.Delete();
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Gets blog entries for the current blog
        /// </summary>
        /// <returns>The entries for the current blog</returns>
        public EntryItem[] GetBlogEntries()
        {
            return GetBlogEntries(Context.Item, int.MaxValue, null, null);
        }

        /// <summary>
        /// Gets blog entries for the current blog up to the maximum number given
        /// </summary>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the current blog</returns>
        public EntryItem[] GetBlogEntries(int maxNumber)
        {
            return GetBlogEntries(Context.Item, maxNumber, null, null);
        }

        /// <summary>
        /// Gets blog entries for the current blog containing the given tag
        /// </summary>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the current blog containing the given tag</returns>
        public EntryItem[] GetBlogEntries(string tag)
        {
            return GetBlogEntries(Context.Item, int.MaxValue, tag, null);
        }

        /// <summary>
        /// Gets blog entries for the current blog containing the given tag up to the maximum number given
        /// </summary>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the current blog containing the given tag</returns>
        public EntryItem[] GetBlogEntries(int maxNumber, string tag)
        {
            return GetBlogEntries(Context.Item, maxNumber, tag, null);
        }

        /// <summary>
        /// Gets blog entries for the given blog
        /// </summary>
        /// <param name="blogItem">The blog item to retrieve the entries for</param>
        /// <returns>The entries for the current blog</returns>
        public EntryItem[] GetBlogEntries(Item blogItem)
        {
            return GetBlogEntries(blogItem, int.MaxValue, null, null);
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the given blog</returns>
        public EntryItem[] GetBlogEntries(ID blogID, int maxNumber)
        {
            return GetBlogEntries(blogID, Context.Database, maxNumber);
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="database">The database to get the blog from</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the given blog</returns>
        public EntryItem[] GetBlogEntries(ID blogID, Database database, int maxNumber)
        {
            var blog = database.GetItem(blogID);
            if (blog != null)
                return GetBlogEntries(blog, maxNumber, null, null);
            else
                return new EntryItem[0];
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the given blog</returns>
        public EntryItem[] GetBlogEntries(ID blogID, int maxNumber, string tag)
        {
            return GetBlogEntries(blogID, Context.Database, maxNumber, tag);
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="database">The database to get the blog from</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the given blog</returns>
        public EntryItem[] GetBlogEntries(ID blogID, Database database, int maxNumber, string tag)
        {
            var blog = database.GetItem(blogID);
            if (blog != null)
                return GetBlogEntries(blog, maxNumber, tag, null);
            else
                return new EntryItem[0];
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blog">The blog item to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <param name="category">A category the entry must contain</param>
        /// <returns></returns>
        public EntryItem[] GetBlogEntries(Item blog, int maxNumber, string tag, string category)
        {
            if (blog != null && maxNumber > 0)
            {
                var query = new CombinedQuery();
                //query.Add(new FieldQuery(Constants.Index.Fields.BlogID, blog.ID.ToShortID().ToString().ToLower()), QueryOccurance.Must);
                query.Add(new FieldQuery(Sitecore.Search.BuiltinFields.Path, blog.ID.ToShortID().ToString()), QueryOccurance.Must);

                // TODO: What about items using templates derived from entryemplateid? need to accommodate those
                query.Add(new FieldQuery(Constants.Index.Fields.Template, Settings.EntryTemplateId.ToShortID().ToString().ToLower()), QueryOccurance.Must);

                if(!string.IsNullOrEmpty(tag))
                    query.Add(new FieldQuery(Constants.Index.Fields.Tags, Utilities.Search.TransformCSV(tag)), QueryOccurance.Must);

                if(!string.IsNullOrEmpty(category))
                {
                    var categoryItem = ManagerFactory.CategoryManagerInstance.GetCategory(blog, category);
                    ID id = ID.Null;

                    if (categoryItem != null)
                        id = categoryItem.ID;

                    query.Add(new FieldQuery(Constants.Index.Fields.Category, id.ToShortID().ToString().ToLower()), QueryOccurance.Must);
                }

                var result = Utilities.Search.Execute<EntryItem>(query, maxNumber, (list, item) => list.Add((EntryItem)item), Constants.Index.Fields.Created, true);

                return result;
            }

            //return blogPostList.ToArray();
            return new EntryItem[0];
        }

        /// <summary>
        /// Gets the blog entries for a particular month and year.
        /// </summary>
        /// <param name="month">The month to get the entries for</param>
        /// <param name="year">The year to get the entries for</param>
        /// <returns>The entries for the month and year from the current blog</returns>
        public EntryItem[] GetBlogEntriesByMonthAndYear(int month, int year)
        {
            var blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog();
            return GetBlogEntriesByMonthAndYear(blog.InnerItem, month, year);
        }

        /// <summary>
        /// Gets the blog entries for a particular month and year.
        /// </summary>
        /// <param name="month">The month to get the entries for</param>
        /// <param name="year">The year to get the entries for</param>
        /// <returns>The entries for the month and year from the current blog</returns>
        public EntryItem[] GetBlogEntriesByMonthAndYear(Item blog, int month, int year)
        {
            if (month >= 13)
                return new EntryItem[0];

            // NewsMover places entries in year, month and day folders. We can rely on this to find entries by time
            // TODO: What about if year and month folder are missing?
            var monthName = Thread.CurrentThread.CurrentCulture.DateTimeFormat.GetMonthName(month);
            var entryTemplateID = Settings.EntryTemplateIdString;
            var entries = blog.Axes.SelectItems("./" + year.ToString() + "/" + monthName + "//*[@@templateid='{0}']".FormatWith(entryTemplateID));

            if (entries == null) return new EntryItem[0];

            return (from entry in entries select (EntryItem)entry).ToArray();
        }

        /// <summary>
        /// Gets the blog entry by categorie.
        /// </summary>
        /// <param name="blogId">The blog ID.</param>
        /// <param name="categorieName">Name of the categorie.</param>
        /// <returns>The entries of the blog tagged with the category</returns>
        public EntryItem[] GetBlogEntryByCategorie(ID blogId, string categorieName)
        {
            var categories = ManagerFactory.CategoryManagerInstance.GetCategories(blogId.ToString());
            if (categories != null)
            {
                var category = (from cat in categories
                                where string.Compare(cat.Name, categorieName, true) == 0
                                select cat).FirstOrDefault();

                if (category != null)
                {
                    return GetBlogEntryByCategorie(blogId, category.ID);
                }
            }

            return new EntryItem[0];
        }

        /// <summary>
        /// Gets the blog entry by categorie.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <param name="CategorieName">Name of the categorie.</param>
        /// <returns></returns>
        public EntryItem[] GetBlogEntryByCategorie(ID blogId, ID categoryId)
        {
            var blogItem = Sitecore.Context.Database.GetItem(blogId);
            var categoryItem = Sitecore.Context.Database.GetItem(categoryId);

            if (blogItem != null && categoryItem != null)
                return GetBlogEntries(blogItem, int.MaxValue, string.Empty, categoryItem.Name);
            else
                return new EntryItem[0];
        }

        /// <summary>
        /// Gets the most popular entries for the blog by the number of page views
        /// </summary>
        /// <param name="blogItem">The blog to find the most popular pages for</param>
        /// <param name="maxCount">The maximum number of entries to return</param>
        /// <returns>An array of EntryItem classes</returns>
        public EntryItem[] GetPopularEntriesByView(Item blogItem, int maxCount)
        {
            var entryIds = from entry in GetBlogEntries(blogItem) select entry.ID.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
            var sql = "select {{0}}ItemId{{1}} from $page_table$ where itemid in ('{0}') group by {{0}}ItemId{{1}} order by count({{0}}ItemId{{1}}) desc".FormatWith(string.Join("','", entryIds.ToArray()));                     
            
            if (entryIds.Count() > 0)
            {
#if PRE_65
                sql = sql.Replace("$page_table$", "page");
                var ids = AnalyticsManager.ReadMany<ID>(sql, reader =>
                {
                    return new ID(AnalyticsManager.GetGuid(0, reader));
                }, new object[0]);
#else
                sql = sql.Replace("$page_table$", "pages");
                var ids = DataAdapterManager.ReportingSql.ReadMany<ID>(sql, reader =>
                {
                    return new ID(DataAdapterManager.ReportingSql.GetGuid(0, reader));
                }, new object[0]); 
#endif
                var limitedIds = ids.Take(maxCount).ToArray();
                return (from id in limitedIds select new EntryItem(blogItem.Database.GetItem(id))).ToArray();
            }
            else
                return new EntryItem[0];
        }

        /// <summary>
        /// Gets the most popular entries for the blog by the number of comments on the entry
        /// </summary>
        /// <param name="blogItem">The blog to find the most popular pages for</param>
        /// <param name="maxCount">The maximum number of entries to return</param>
        /// <returns>An array of EntryItem classes</returns>
        public EntryItem[] GetPopularEntriesByComment(Item blogItem, int maxCount)
        {
            var comments = ManagerFactory.CommentManagerInstance.GetCommentsByBlog(blogItem, int.MaxValue);
            var grouped = from comment in comments
                          group comment by GetBlogEntryByComment(comment).ID into g
                          orderby g.Count() descending
                          select g.Key;

            var ids = grouped.Take(maxCount).ToArray();
            return (from id in ids select new EntryItem(blogItem.Database.GetItem(id))).ToArray();
        }

        /// <summary>
        /// Gets the entry item for the current comment.
        /// </summary>
        /// <param name="commentItem">The comment item.</param>
        /// <returns></returns>
        public EntryItem GetBlogEntryByComment(CommentItem commentItem)
        {
            Item[] blogEntry = commentItem.InnerItem.Axes.GetAncestors().Where(item => item.TemplateID == Settings.EntryTemplateId).ToArray();

            if (blogEntry.Length > 0)
            {
                return new EntryItem(blogEntry.FirstOrDefault());
            }
            return null;
        }

        /// <summary>
        /// Makes the sorted post item list.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public EntryItem[] MakeSortedEntriesList(IList array)
        {
            var postItemList = new List<EntryItem>();
            var template = Sitecore.Context.Database.GetTemplate(Settings.EntryTemplateId);

            if(template != null)
            {
                foreach (Item item in array)
                {
                    if (Utilities.Items.TemplateIsOrBasedOn(item, template) && item.Versions.GetVersions().Length > 0)
                    {
                        postItemList.Add(new EntryItem(item));
                    }
                }

                postItemList.Sort(new PostDateComparerDesc());
            }

            return postItemList.ToArray();
        }

        /*#region Obsolete Methods
        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="postId">The ID of the post to delete</param>
        /// <returns>True if the post was deleted, otherwise False</returns>
        [Obsolete("Use DeleteEntry(string postID) instead")]
        public static bool DeletePost(string postId)
        {
            return new EntryManager().DeleteEntry(postId);
        }

        [Obsolete("Use GetBlogEntries(int maxNumber) instead")]
        public static EntryItem[] GetAllEntries(int maxNumber)
        {
            return new EntryManager().GetBlogEntries(maxNumber);
        }

        /// <summary>
        /// Gets all blog posts.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <param name="MaxNumber">The max number.</param>
        /// <returns></returns>
        [Obsolete("Use GetBlogEntries(ID BlogID, int MaxNumber).InnerItem instead")]
        public static Item[] GetBlogEntriesAsItems(ID blogId, int maxNumber)
        {
            return (from entry in new EntryManager().GetBlogEntries(blogId, maxNumber) select entry.InnerItem).ToArray();
        }

        /// <summary>
        /// Gets the blog entries.
        /// </summary>
        /// <param name="Tag">The tag.</param>
        /// <returns></returns>
        [Obsolete("Use GetBlogEntries(string).InnerItem instead")]
        public static Item[] GetBlogEntryItems(string tag)
        {
            return (from entry in new EntryManager().GetBlogEntries(tag) select entry.InnerItem).ToArray();
        }

        /// <summary>
        /// Gets the blog entries.
        /// </summary>
        /// <param name="Tag">The tag.</param>
        /// <param name="MaxNumber">The max number.</param>
        /// <returns></returns>
        [Obsolete("Use GetBlogEntries(int maxNumber, string tag) instead")]
        public static EntryItem[] GetBlogEntries(string tag, int maxNumber)
        {
            return new EntryManager().GetBlogEntries(maxNumber, tag);
        }

        /// <summary>
        /// Gets the blog posty by ID.
        /// </summary>
        /// <param name="BlogPostID">The blog post ID.</param>
        /// <returns></returns>
        [Obsolete("Retrieve the item directly from the databse")]
        public static Item GetBlogEntryByID(ID blogPostId)
        {
            var entry = Context.Database.GetItem(blogPostId);
            if (entry == null)
                Log.Error("Could not find blog item:" + blogPostId, typeof(EntryManager));

            return entry;
        }

        /// <summary>
        /// Gets the blog posty by ID.
        /// </summary>
        /// <param name="BlogPostID">The blog post ID.</param>
        /// <returns></returns>
        [Obsolete("Create a new Entry instance from the item given by the ID")]
        public static EntryItem GetBlogEntry(ID blogPostId)
        {
            var entry = GetBlogEntryByID(blogPostId);
            if (entry != null)
                return new EntryItem(entry);
            else
                return null;
        }

        /// <summary>
        /// Gets the blog entry by categorie.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <param name="CategorieID">ID of the categorie.</param>
        /// <returns></returns>
        [Obsolete("Use GetBlogEntryByCategorie().InnerItem instead")]
        public static Item[] GetBlogEntryByCategorieAsItem(ID blogID, string categorieId)
        {
            var entries = new EntryManager().GetBlogEntryByCategorie(blogID, categorieId);
            return (from entry in entries
                    select entry.InnerItem).ToArray();
        }

        /// <summary>
        /// Makes the sorted post item list.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        [Obsolete("Use MakeSortedEntriesList().InnerItem instead")]
        public static Item[] MakeSortedItemList(IList array)
        {
            var sortedList = new EntryManager().MakeSortedEntriesList(array);
            return (from entry in sortedList
                    select entry.InnerItem).ToArray();
        }
        #endregion*/
    }
}