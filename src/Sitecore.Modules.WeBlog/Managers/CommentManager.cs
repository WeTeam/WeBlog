using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Eventing;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Search.SearchTypes;
using Sitecore.Modules.WeBlog.Services;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with comments
    /// </summary>
    public class CommentManager : ICommentManager
    {
        /// <summary>
        /// The settings to use.
        /// </summary>
        protected IWeBlogSettings Settings = null;

        /// <summary>
        /// Gets the <see cref="BaseTemplateManager"/> used to access templates.
        /// </summary>
        protected BaseTemplateManager TemplateManager { get; }

        /// <summary>
        /// Gets the <see cref="IBlogSettingsResolver"/> used to resolve the settings for a given blog item.
        /// </summary>
        protected IBlogSettingsResolver BlogSettingsResolver { get; }

        /// <summary>
        /// Gets the <see cref="IEventQueue"/> which new comments are submitted to.
        /// </summary>
        protected IEventQueue EventQueue { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="settings">The settings to use, or pass null to use the default settings.</param>
        /// <param name="templateManager">The <see cref="BaseTemplateManager"/> used to access templates.</param>
        /// <param name="blogSettingsResolver">The resolver used to resolve settings for specific blogs.</param>
        [Obsolete("Use ctor(IWeBlogSettings, BaseTemplateManager, IBlogSettingsResolver, BaseEventQueueProvider) instead.")]
        public CommentManager(IWeBlogSettings settings = null, BaseTemplateManager templateManager = null, IBlogSettingsResolver blogSettingsResolver = null)
        {
            Settings = settings ?? WeBlogSettings.Instance;
            TemplateManager = templateManager ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseTemplateManager>();
            BlogSettingsResolver = blogSettingsResolver ?? ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>();
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="settings">The settings to use, or pass null to use the default settings.</param>
        /// <param name="templateManager">The <see cref="BaseTemplateManager"/> used to access templates.</param>
        /// <param name="blogSettingsResolver">The resolver used to resolve settings for specific blogs.</param>
        /// <param name="eventQueue">The event queue to submit new comments to.</param>
        public CommentManager(IWeBlogSettings settings = null, BaseTemplateManager templateManager = null, IBlogSettingsResolver blogSettingsResolver = null, BaseEventQueueProvider eventQueue = null)
        {
            Settings = settings ?? WeBlogSettings.Instance;
            TemplateManager = templateManager ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseTemplateManager>();
            BlogSettingsResolver = blogSettingsResolver ?? ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>();
            EventQueue = eventQueue ?? ServiceLocator.ServiceProvider.GetRequiredService<BaseEventQueueProvider>();
        }

        /// <summary>
        /// Adds a comment to a blog
        /// </summary>
        /// <param name="entryId">The ID of the entry to add the comment to</param>
        /// <param name="comment">The comment to add to the entry</param>
        /// <param name="language">The language to create the comment in</param>
        /// <returns>The ID of the created comment item, or ID.Null if creation failed</returns>
        public virtual ID AddCommentToEntry(ID entryId, Model.Comment comment, Language language = null)
        {
            var args = new CreateCommentArgs();
            args.EntryID = entryId;
            args.Comment = comment;
            args.Database = ContentHelper.GetContentDatabase();
            args.Language = language ?? Context.Language;

            CorePipeline.Run("weblogCreateComment", args, true);

            if (args.CommentItem != null)
                return args.CommentItem.ID;
            else
                return ID.Null;
        }

        /// <summary>
        /// Submit a comment for inclusion on a post. This method will either update Sitecore or submit the comment through the comment service, depending on settings
        /// </summary>
        /// <param name="entryId">The ID of the entry to add the comment to</param>
        /// <param name="comment">The comment to add to the entry</param>
        /// <param name="language">The language to create the comment in</param>
        /// <returns>The ID of the created comment item, or ID.Null if creation failed</returns>
        public virtual ID SubmitComment(ID entryId, Model.Comment comment, Language language = null)
        {
            if (Settings.CommentServiceEnabled)
            {
                // Submit comment through WCF service
                ChannelFactory<ICommentService> commentProxy = new ChannelFactory<ICommentService>("WeBlogCommentService");
                commentProxy.Open();
                ICommentService service = commentProxy.CreateChannel();
                var result = service.SubmitComment(Context.Item.ID, comment, language);
                commentProxy.Close();
                if (result == ID.Null)
                    Logger.Error("Comment submission through WCF failed. Check server Sitecore log for details", typeof(CommentManager));
                return result;
            }
            else
            {
                var commentEvent = new CommentSubmitted
                {
                    EntryId = entryId,
                    RequestedCommentId = ID.NewID,
                    Language = language ?? Context.Language,
                    Comment = comment
                };

                EventQueue.QueueEvent(commentEvent, true, Settings.HandleSubmittedCommentsLocally);

                return commentEvent.RequestedCommentId;
            }
        }

        /// <summary>
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entry">The entry to get the comment count for</param>
        /// <returns>The number of comments</returns>
        public virtual int GetCommentsCount(Item entry)
        {
            return GetEntryComments(entry, int.MaxValue).Count();
        }

        /// <summary>
        /// Gets the comments for the blog entry
        /// </summary>
        /// <param name="blogItem">The blog to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        public virtual IList<CommentReference> GetBlogComments(Item blogItem, int maximumCount)
        {
            return GetCommentsFor(blogItem, maximumCount, true);
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        public virtual IList<CommentReference> GetEntryComments(Item entryItem, int maximumCount)
        {
            if (entryItem != null)
            {
                if(TemplateManager.TemplateIsOrBasedOn(entryItem, Settings.EntryTemplateIds))
                { 
                    return GetCommentsFor(entryItem, maximumCount);
                }
            }

            return new CommentReference[0];
        }

        /// <summary>
        /// Get the <see cref="CommentContent"/> for a <see cref="CommentReference"/>.
        /// </summary>
        /// <param name="commentReference">The <see cref="CommentReference"/> which identifies the comment.</param>
        /// <returns>The <see cref="CommentContent"/> for the <see cref="CommentReference"/>.</returns>
        public virtual CommentContent GetCommentContent(CommentReference commentReference)
        {
            CommentItem commentItem = Database.GetItem(commentReference.Uri);

            return commentItem;
        }

        /// <summary>
        /// Gets the <see cref="ItemUri"/>s for the entries that have the most comments.
        /// </summary>
        /// <param name="item">The root blog item to search below.</param>
        /// <param name="maximumCount">The maximum number of entry <see cref="ItemUri"/>s to return.</param>
        /// <returns>The list of <see cref="ItemUri"/>s for the entries.</returns>
        public IList<ItemUri> GetMostCommentedEntries(Item item, int maximumCount)
        {
            if (item == null || maximumCount <= 0)
                return new ItemUri[0];

            var results = SearchComments<ItemUri>(item, queryable =>
            {
                if (!queryable.Any())
                    return new ItemUri[0];

                var facets = queryable.FacetOn(x => x.EntryUri);
                var facetResults = facets.GetFacets();

                if (!facetResults.Categories.Any())
                    return new ItemUri[0];

                var orderedRawUris = facetResults.Categories[0].Values.OrderByDescending(x => x.AggregateCount).Where(x => x.AggregateCount != 0).Take(maximumCount).ToList();
                var parsedUris = orderedRawUris.Select(x => ItemUri.Parse(x.Name));
                return parsedUris.ToList();
            });

            return results;
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="item">The item to get the comments under</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="language">The language to get the comments in</param>
        /// <returns>The comments which are decendants of the given item</returns>
        protected virtual IList<CommentReference> GetCommentsFor(Item item, int maximumCount, bool reverseSort = false)
        {
            if (item == null || maximumCount <= 0)
                return new CommentReference[0];

            var results = SearchComments<CommentReference>(item, queryable =>
            {
                if (!queryable.Any())
                    return new CommentReference[0];
                
                if (reverseSort)
                    queryable = queryable.OrderByDescending(x => x.FullCreatedDate);
                else
                    queryable = queryable.OrderBy(x => x.FullCreatedDate);

                queryable = queryable.Take(maximumCount);

                return queryable.Select(x => CreateCommentReference(x)).ToList();

                // todo: need cache?
            });

            return results;
        }

        /// <summary>
        /// Search for comments.
        /// </summary>
        /// <param name="item">The root item under which to search for comments.</param>
        /// <param name="projection">The function used to project the search results.</param>
        /// <returns>A list of the found comments.</returns>
        protected virtual IList<T> SearchComments<T>(Item item, Func<IQueryable<CommentResultItem>, IList<T>> projection)
        {
            if (item == null)
                return new T[0];

            var blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(item);
            if (blog == null)
                return new T[0];

            var indexName = Settings.SearchIndexName;

            if (string.IsNullOrEmpty(indexName))
                return new T[0];

            var index = ContentSearchManager.GetIndex(indexName + "-" + item.Database.Name);
            if (index == null)
                return new T[0];

            using (var context = index.CreateSearchContext())
            {
                var queryable = CreateQueryable(context, item, blog);
                return projection(queryable);
            }
        }

        protected virtual IQueryable<CommentResultItem> CreateQueryable(IProviderSearchContext context, Item rootItem, BlogHomeItem blogItem)
        {
            var settings = BlogSettingsResolver.Resolve(blogItem);

            return context.GetQueryable<CommentResultItem>().Where(x =>
                x.Paths.Contains(rootItem.ID) &&
                x.TemplateId == settings.CommentTemplateID &&
                x.Language == rootItem.Language.Name
            );
        }

        protected virtual CommentReference CreateCommentReference(CommentResultItem resultItem)
        {
            return new CommentReference
            {
                Uri = resultItem.Uri,
                EntryUri = resultItem.EntryUri,
                Created = resultItem.CreatedDate
            };
        }
    }
}
