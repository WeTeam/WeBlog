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
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Search.SearchTypes;
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
        /// <summary>
        /// The settings to use.
        /// </summary>
        protected IWeBlogSettings Settings = null;

#if FEATURE_XDB
        /// <summary>The <see cref="ReportDataProviderBase"/> to read reporting data from.</summary>
        protected ReportDataProviderBase ReportDataProvider = null;

        public EntryManager()
            : this(null, null)
        {
        }

        public EntryManager(ReportDataProviderBase reportDataProvider, IWeBlogSettings settings)
        {
            ReportDataProvider = reportDataProvider;
            Settings = settings ?? new WeBlogSettings();
        }
#else
        public EntryManager()
            : this(null)
        {
        }

        public EntryManager(IWeBlogSettings settings)
        {
            Settings = settings ?? new WeBlogSettings();
        }
#endif

        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="postID">The ID of the post to delete</param>
        /// <param name="db">The database to delete the entry from</param>
        /// <returns>True if the post was deleted, otherwise False</returns>
        public virtual bool DeleteEntry(string postID, Database db)
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
        /// Gets blog entries for the given blog
        /// </summary>
        /// <param name="blogItem">The blog item to retrieve the entries for</param>
        /// <returns>The entries for the current blog</returns>
        public virtual EntryItem[] GetBlogEntries(Item blogItem)
        {
            return GetBlogEntries(blogItem, int.MaxValue, null, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="database">The database to get the blog from</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the given blog</returns>
        public virtual EntryItem[] GetBlogEntries(ID blogID, Database database, int maxNumber)
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
        /// <param name="database">The database to get the blog from</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the given blog</returns>
        public virtual EntryItem[] GetBlogEntries(ID blogID, Database database, int maxNumber, string tag)
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
        /// <param name="minimumDate">The minimum date for entries</param>
        /// <param name="maximumDate">The maximum date for the entries</param>
        /// <returns></returns>
        public virtual EntryItem[] GetBlogEntries(Item blog, int maxNumber, string tag, string category, DateTime? minimumDate = null, DateTime? maximumDate = null)
        {
            if (blog == null || maxNumber <= 0)
            {
                return new EntryItem[0];
            }

            var customBlogItem = (from templateId in Settings.BlogTemplateIds
                where blog.TemplateIsOrBasedOn(templateId)
                select (BlogHomeItem)blog).FirstOrDefault();

            if (customBlogItem == null)
            {
                customBlogItem = (from templateId in Settings.BlogTemplateIds
                                 let item = blog.FindAncestorByTemplate(templateId)
                                 where item != null
                                 select (BlogHomeItem)item).FirstOrDefault();
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
                        builder = builder.And(i => i.EntryDate >= minimumDate);

                    if (maximumDate != null)
                        builder = builder.And(i => i.EntryDate < maximumDate);

                    var indexresults = context.GetQueryable<EntryResultItem>().Where(builder);

                    if (indexresults.Any())
                    {
                        var itemResults = indexresults.Select(indexresult => indexresult.GetItem()).ToList();
                        result = itemResults.Where(item => item != null).Select(i => new EntryItem(i)).ToList();
                        result = result.OrderByDescending(post => post.EntryDate.DateTime).ThenBy(post => post.Created).Take(maxNumber).ToList();
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
        public virtual EntryItem[] GetBlogEntriesByMonthAndYear(Item blog, int month, int year)
        {
            if (month >= 13)
                return new EntryItem[0];

            var minDate = new DateTime(year, month, 1);
            var maxDate = minDate.AddMonths(1);

            return GetBlogEntries(blog, int.MaxValue, null, null, minDate, maxDate);
        }

        /// <summary>
        /// Gets the most popular entries for the blog by the number of page views
        /// </summary>
        /// <param name="blogItem">The blog to find the most popular pages for</param>
        /// <param name="maxCount">The maximum number of entries to return</param>
        /// <returns>An array of EntryItem classes</returns>
        public virtual EntryItem[] GetPopularEntriesByView(Item blogItem, int maxCount)
        {
            if (AnalyticsEnabled())
            {
                var blogEntries = GetBlogEntries(blogItem);
                var entryIds = (from entry in blogEntries select entry.ID).ToArray();

                if (entryIds.Any())
                {
                    var views = new Dictionary<ID, long>();

                    foreach (var id in entryIds)
                    {
#if FEATURE_XDB
                        var query = new ItemVisitsQuery(this.ReportDataProvider)
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
            }
            return new EntryItem[0];
        }

        protected static bool AnalyticsEnabled()
        {
            return Sitecore.Configuration.Settings.GetBoolSetting("Analytics.Enabled", false) ||
                   Sitecore.Configuration.Settings.GetBoolSetting("Xdb.Enabled", false);
        }

        /// <summary>
        /// Gets the most popular entries for the blog by the number of comments on the entry
        /// </summary>
        /// <param name="blogItem">The blog to find the most popular pages for</param>
        /// <param name="maxCount">The maximum number of entries to return</param>
        /// <returns>An array of EntryItem classes</returns>
        public virtual EntryItem[] GetPopularEntriesByComment(Item blogItem, int maxCount)
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
        public virtual EntryItem GetBlogEntryByComment(CommentItem commentItem)
        {
            if (commentItem == null)
                return null;

            return commentItem.InnerItem.FindAncestorByAnyTemplate(Settings.EntryTemplateIds);
        }

        #region Deprecated

        /// <summary>
        /// Gets the current context item as a blog entry
        /// </summary>
        /// <returns>The current blog entry</returns>
        [Obsolete("Use EntryItem class instead.")] // deprecated 3.0
        public virtual EntryItem GetCurrentBlogEntry()
        {
            var current = new EntryItem(Context.Item);
            return current;
        }
        
        /// <summary>
        /// Gets the current context item as a blog entry
        /// </summary>
        /// <param name="item">The item to find the current entry item for</param>
        /// <returns>The current blog entry</returns>
        [Obsolete("Use EntryItem class instead.")] // deprecated 3.0
        public virtual EntryItem GetCurrentBlogEntry(Item item)
        {
            return (from templateId in Settings.EntryTemplateIds
                let entryItem = item.FindAncestorByTemplate(templateId)
                where entryItem != null
                select new EntryItem(entryItem)).FirstOrDefault();
        }

        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="postID">The ID of the post to delete</param>
        /// <returns>True if the post was deleted, otherwise False</returns>
        [Obsolete("Use DeleteEntry(string, Database) instead.")] // deprecated 3.0
        public virtual bool DeleteEntry(string postID)
        {
            return DeleteEntry(postID, Sitecore.Context.Database);
        }

        /// <summary>
        /// Gets blog entries for the current blog
        /// </summary>
        /// <returns>The entries for the current blog</returns>
        [Obsolete("Use GetBlogEntries(Item) instead")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntries()
        {
            return GetBlogEntries(Context.Item, int.MaxValue, null, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the current blog up to the maximum number given
        /// </summary>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the current blog</returns>
        [Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntries(int maxNumber)
        {
            return GetBlogEntries(Context.Item, maxNumber, null, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the current blog containing the given tag
        /// </summary>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the current blog containing the given tag</returns>
        [Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntries(string tag)
        {
            return GetBlogEntries(Context.Item, int.MaxValue, tag, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the current blog containing the given tag up to the maximum number given
        /// </summary>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the current blog containing the given tag</returns>
        [Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntries(int maxNumber, string tag)
        {
            return GetBlogEntries(Context.Item, maxNumber, tag, null, (DateTime?)null);
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the given blog</returns>
        [Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntries(ID blogID, int maxNumber)
        {
            return GetBlogEntries(blogID, Context.Database, maxNumber);
        }

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the given blog</returns>
        [Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntries(ID blogID, int maxNumber, string tag)
        {
            return GetBlogEntries(blogID, Context.Database, maxNumber, tag);
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
        public virtual EntryItem[] GetBlogEntries(Item blog, int maxNumber, string tag, string category, string datePrefix = null)
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
        /// Gets the blog entries for a particular month and year.
        /// </summary>
        /// <param name="month">The month to get the entries for</param>
        /// <param name="year">The year to get the entries for</param>
        /// <returns>The entries for the month and year from the current blog</returns>
        [Obsolete("Use GetBlogEntriesByMonthAndYear(Item, int, int) instead.")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntriesByMonthAndYear(int month, int year)
        {
            return GetBlogEntriesByMonthAndYear(ManagerFactory.BlogManagerInstance.GetCurrentBlog(), month, year);
        }

        /// <summary>
        /// Gets the blog entry by categorie.
        /// </summary>
        /// <param name="blogId">The blog ID.</param>
        /// <param name="categorieName">Name of the categorie.</param>
        /// <returns>The entries of the blog tagged with the category</returns>
        [Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntryByCategorie(ID blogId, string categorieName)
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
        [Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        public virtual EntryItem[] GetBlogEntryByCategorie(ID blogId, ID categoryId)
        {
            var blogItem = Sitecore.Context.Database.GetItem(blogId);
            var categoryItem = Sitecore.Context.Database.GetItem(categoryId);

            if (blogItem != null && categoryItem != null)
                return GetBlogEntries(blogItem, int.MaxValue, string.Empty, categoryItem.Name, (DateTime?)null);
            else
                return new EntryItem[0];
        }

        /// <summary>
        /// Makes the sorted post item list.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        [Obsolete("No longer used.")] // deprecated 3.0
        public virtual EntryItem[] MakeSortedEntriesList(IList array)
        {
            var entryList = (from Item item in array
                where item.TemplateIsOrBasedOn(Settings.EntryTemplateIds)
                && item.Versions.Count > 0
                select new EntryItem(item)).ToList();

            entryList.Sort(new PostDateComparerDesc());

            return entryList.ToArray();
        }

        #endregion
    }
}