using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Comparers;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.Security;
using Sitecore.Modules.WeBlog.Search.SearchTypes;
using Sitecore.StringExtensions;
#if FEATURE_XDB
using Sitecore.Modules.WeBlog.Analytics.Reporting;
using Sitecore.Analytics.Reporting;
#elif FEATURE_DMS
using Sitecore.Analytics.Data.DataAccess.DataAdapters;
#else
using Sitecore.Analytics.Reports.Data.DataAccess.DataAdapters;
#endif

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with blog entries
    /// </summary>
    public class EntryManager : IEntryManager
    {
#if FEATURE_XDB
        /// <summary>The <see cref="ReportDataProviderBase"/> to read reporting data from.</summary>
        private ReportDataProviderBase reportDataProvider = null;

        public EntryManager(ReportDataProviderBase reportDataProvider = null)
        {
            this.reportDataProvider = reportDataProvider;
        }
#else
        public EntryManager()
        {
        }
#endif

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
            var entryItem = item.GetCurrentItem(Settings.EntryTemplateIDString);

            if (entryItem != null)
                return new EntryItem(entryItem);
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
            return GetBlogEntries(Context.Item, int.MaxValue, null, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the current blog up to the maximum number given
        /// </summary>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the current blog</returns>
        public EntryItem[] GetBlogEntries(int maxNumber)
        {
            return GetBlogEntries(Context.Item, maxNumber, null, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the current blog containing the given tag
        /// </summary>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the current blog containing the given tag</returns>
        public EntryItem[] GetBlogEntries(string tag)
        {
            return GetBlogEntries(Context.Item, int.MaxValue, tag, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the current blog containing the given tag up to the maximum number given
        /// </summary>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the current blog containing the given tag</returns>
        public EntryItem[] GetBlogEntries(int maxNumber, string tag)
        {
            return GetBlogEntries(Context.Item, maxNumber, tag, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the given blog
        /// </summary>
        /// <param name="blogItem">The blog item to retrieve the entries for</param>
        /// <returns>The entries for the current blog</returns>
        public EntryItem[] GetBlogEntries(Item blogItem)
        {
            return GetBlogEntries(blogItem, int.MaxValue, null, null, (DateTime?)null);
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
                return GetBlogEntries(blog, maxNumber, null, null, (DateTime?)null);
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
                return GetBlogEntries(blog, maxNumber, tag, null, (DateTime?)null);
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
        [Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?) instead")] // deprecated in 2.4
        public EntryItem[] GetBlogEntries(Item blog, int maxNumber, string tag, string category, string datePrefix = null)
        {
            if (datePrefix.Length != 6)
                return new EntryItem[0];

            // First four digits should be year
            var yearStr = datePrefix.Substring(0, 4);
            var year = 0;
            if (!int.TryParse(yearStr, out year))
                return new EntryItem[0];

            // Next two digits should be month
            var monthStr = datePrefix.Substring(4, 2);
            var month = 0;
            if (!int.TryParse(monthStr, out month))
                return new EntryItem[0];

            if (month >= 13)
                return new EntryItem[0];

            return GetBlogEntries(blog, Int32.MaxValue, null, null, new DateTime(year, month, 1));
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blog">The blog item to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <param name="category">A category the entry must contain</param>
        /// <param name="minimumDate">The minimum date for entries</param>
        /// <param name="maximumDate">The maximum date for the entries</param>
        /// <returns></returns>
        public EntryItem[] GetBlogEntries(Item blog, int maxNumber, string tag, string category, DateTime? minimumDate = null, DateTime? maximumDate = null)
        {
            if (blog == null || maxNumber <= 0)
            {
                return new EntryItem[0];
            }

            BlogHomeItem customBlogItem = null;
            if (blog.TemplateIsOrBasedOn(Settings.BlogTemplateID))
            {
                customBlogItem = blog;
            }
            else
            {
                customBlogItem = blog.GetCurrentItem(Settings.BlogTemplateIDString);
            }
            if (customBlogItem == null)
            {
                return new EntryItem[0];
            }


            List<EntryItem> result = new List<EntryItem>();
            var indexName = Settings.SearchIndexName;

            if (!string.IsNullOrEmpty(indexName))
            {

                using (var context = ContentSearchManager.GetIndex(indexName).CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
                {
                    var builder = PredicateBuilder.True<EntryResultItem>();

                    var id = customBlogItem.BlogSettings.EntryTemplateID;
                    builder = builder.And(i => i.TemplateId == id);
                    builder = builder.And(i => i.Paths.Contains(customBlogItem.ID));
                    builder = builder.And(i => i.Language.Equals(customBlogItem.InnerItem.Language.Name, StringComparison.InvariantCulture));
                    builder = builder.And(item => item.DatabaseName.Equals(Context.Database.Name, StringComparison.InvariantCulture));

                    // Tag
                    if (!string.IsNullOrEmpty(tag))
                    {
                        builder = builder.And(i => i.Tags.Contains(tag));
                    }

                    // Categories
                    if (!string.IsNullOrEmpty(category))
                    {
                        var categoryItem = ManagerFactory.CategoryManagerInstance.GetCategory(customBlogItem, category);

                        // If the category is unknown, don't return any results.
                        if (categoryItem == null)
                            return new EntryItem[0];
#if SC70
                        var normalizedID = Sitecore.ContentSearch.Utilities.IdHelper.NormalizeGuid(categoryItem.ID);
                        builder = builder.And(i => i.Category.Contains(normalizedID));
#else
                        builder = builder.And(i => i.Category.Contains(categoryItem.ID));
#endif

                    }

                    if (minimumDate != null)
                        builder = builder.And(i => i.CreatedDate >= minimumDate);

                    if (maximumDate != null)
                        builder = builder.And(i => i.CreatedDate < maximumDate);

                    var indexresults = context.GetQueryable<EntryResultItem>().Where(builder);

                    if (indexresults.Any())
                    {
                        result = indexresults.Select(i => new EntryItem(i.GetItem())).ToList();
                        result = result.OrderByDescending(post => post.EntryDate.DateTime).Take(maxNumber).ToList();
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the blog entries for a particular month and year.
        /// </summary>
        /// <param name="month">The month to get the entries for</param>
        /// <param name="year">The year to get the entries for</param>
        /// <returns>The entries for the month and year from the current blog</returns>
        public EntryItem[] GetBlogEntriesByMonthAndYear(int month, int year)
        {
            return GetBlogEntriesByMonthAndYear(ManagerFactory.BlogManagerInstance.GetCurrentBlog(), month, year);
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

            var minDate = new DateTime(year, month, 1);
            var maxDate = minDate.AddMonths(1);

            return GetBlogEntries(blog, Int32.MaxValue, null, null, minDate, maxDate);
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
                return GetBlogEntries(blogItem, int.MaxValue, string.Empty, categoryItem.Name, (DateTime?)null);
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
            var blogEntries = GetBlogEntries(blogItem);
            var entryIds = (from entry in blogEntries select entry.ID).ToArray();

            if (entryIds.Any())
            {
                var views = new Dictionary<ID, long>();

                foreach (var id in entryIds)
                {
#if FEATURE_XDB
                    var query = new ItemVisitsQuery(this.reportDataProvider)
                    {
                        ItemId = id
                    };

                    query.Execute();

                    var itemViews = query.Visits;
#elif FEATURE_DMS
                    var queryId = id.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
                    var sql = "SELECT COUNT(ItemId) as Visits FROM {{0}}Pages{{1}} WHERE {{0}}ItemId{{1}} = '{0}'".FormatWith(queryId);

                    var itemViews = DataAdapterManager.ReportingSql.ReadOne(sql, reader => DataAdapterManager.ReportingSql.GetLong(0, reader));
#endif

                    if (itemViews > 0)
                        views.Add(id, itemViews);
                }

                var ids = views.OrderByDescending(x => x.Value).Take(maxCount).Select(x => x.Key).ToArray();

                return (from id in ids select blogEntries.First(i => i.ID == id)).ToArray();
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
            Item[] blogEntry = commentItem.InnerItem.Axes.GetAncestors().Where(item => item.TemplateIsOrBasedOn(Settings.EntryTemplateID)).ToArray();

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
            var template = Sitecore.Context.Database.GetTemplate(Settings.EntryTemplateID);

            if (template != null)
            {
                foreach (Item item in array)
                {
                    if (item.TemplateIsOrBasedOn(template) && item.Versions.GetVersions().Length > 0)
                    {
                        postItemList.Add(new EntryItem(item));
                    }
                }

                postItemList.Sort(new PostDateComparerDesc());
            }

            return postItemList.ToArray();
        }
    }
}