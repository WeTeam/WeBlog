using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.Security;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Modules.WeBlog.Search.SearchTypes;
using Sitecore.Modules.WeBlog.Analytics.Reporting;

#if FEATURE_XCONNECT
using Sitecore.Xdb.Reporting;
#else
using Sitecore.Analytics.Reporting;
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

        /// <summary>The <see cref="ReportDataProviderBase"/> to read reporting data from.</summary>
        protected ReportDataProviderBase ReportDataProvider = null;

        public EntryManager()
            : this(null, null)
        {
        }

        public EntryManager(ReportDataProviderBase reportDataProvider, IWeBlogSettings settings = null)
        {
            ReportDataProvider = reportDataProvider;
            Settings = settings ?? WeBlogSettings.Instance;
        }

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

                using (var context = ContentSearchManager.GetIndex(indexName + "-" + blog.Database.Name).CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
                {
                    var builder = PredicateBuilder.True<EntryResultItem>();

                    builder = builder.And(i => i.TemplateId == customBlogItem.BlogSettings.EntryTemplateID);
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

                        builder = builder.And(i => i.Category.Contains(categoryItem.ID));
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
                        result = result.OrderByDescending(post => post.EntryDate.DateTime).ThenByDescending(post => ((Item)post).Statistics.Created).Take(maxNumber).ToList();
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
                        var itemViews = GetItemViews(id);

                        if (itemViews > 0 && !views.ContainsKey(id))
                            views.Add(id, itemViews);
                    }

                    var ids = views.OrderByDescending(x => x.Value).Take(maxCount).Select(x => x.Key).ToArray();

                    return (from id in ids select blogEntries.First(i => i.ID == id)).ToArray();
                }
            }
            else
            {
                Logger.Warn("Sitecore.Analytics must be enabled to get popular entries by view.", this);
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

        /// <summary>
        /// Gets the number of views for the item given by ID.
        /// </summary>
        /// <param name="itemId">The ID of the item to get the views for.</param>
        /// <returns>The number of views for the item.</returns>
        protected virtual long GetItemViews(ID itemId)
        {
            var query = new ItemVisitsQuery(this.ReportDataProvider)
            {
                ItemId = itemId
            };

            query.Execute();

            return query.Visits;
        }
    }
}