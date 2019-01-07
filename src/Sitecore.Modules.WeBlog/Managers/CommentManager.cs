using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Import;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Modules.WeBlog.Pipelines;
using Sitecore.Modules.WeBlog.Search.SearchTypes;
using Sitecore.Modules.WeBlog.Services;
using Sitecore.Pipelines;

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
        /// Creates a new instance.
        /// </summary>
        /// <param name="settings">The settings to use, or pass null to use the default settings.</param>
        public CommentManager(IWeBlogSettings settings = null)
        {
            Settings = settings ?? WeBlogSettings.Instance;
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

        protected virtual void AddComment(EntryItem entryItem, WpComment wpComment)
        {
            // todo: Wizard to ask user which language to import into
            string itemName = ItemUtil.ProposeValidItemName("Comment by " + wpComment.Author + " at " + wpComment.Date.ToString("d"));

            CommentItem commentItem = entryItem.InnerItem.Add(itemName, new TemplateID(new ID(CommentItem.TemplateId)));
            commentItem.BeginEdit();
            commentItem.Comment.Field.Value = wpComment.Content;
            commentItem.Email.Field.Value = wpComment.Email;
            commentItem.IpAddress.Field.Value = wpComment.IP;
            commentItem.Website.Field.Value = wpComment.Url;
            commentItem.InnerItem.Fields[Sitecore.FieldIDs.Created].Value = Sitecore.DateUtil.ToIsoDate(wpComment.Date);
            commentItem.EndEdit();
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
                return AddCommentToEntry(Context.Item.ID, comment, language);
        }

        /// <summary>
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entry">The entry to get the comment count for</param>
        /// <returns>The number of comments</returns>
        public virtual int GetCommentsCount(Item entry)
        {
            return GetEntryComments(entry).Length;
        }

        /// <summary>
        /// Gets the comments for the blog entry
        /// </summary>
        /// <param name="blogItem">The blog to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        public virtual CommentItem[] GetCommentsByBlog(Item blogItem, int maximumCount)
        {
            return GetCommentsFor(blogItem, maximumCount, true, true);
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <returns>The comments for the blog entry</returns>
        public virtual CommentItem[] GetEntryComments(Item entryItem)
        {
            return GetEntryComments(entryItem, int.MaxValue);
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        public virtual CommentItem[] GetEntryComments(Item entryItem, int maximumCount)
        {
            if (entryItem != null)
            {
                if(entryItem.TemplateIsOrBasedOn(Settings.EntryTemplateIds))
                { 
                    return GetCommentsFor(entryItem, maximumCount, true);
                }
            }

            return new CommentItem[0];
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="item">The item to get the comments under</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="language">The language to get the comments in</param>
        /// <returns>The comments which are decendants of the given item</returns>
        public virtual CommentItem[] GetCommentsFor(Item item, int maximumCount, bool sort = false, bool reverse = false)
        {
            if (item != null && maximumCount > 0)
            {
                var blog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(item);
                if (blog != null)
                {
                    var indexName = Settings.SearchIndexName;
                    List<CommentItem> result = new List<CommentItem>();
                    if (!string.IsNullOrEmpty(indexName))
                    {
                        using (var context = ContentSearchManager.GetIndex(indexName + "-" + item.Database.Name).CreateSearchContext())
                        {
                            var indexresults = context.GetQueryable<CommentResultItem>().Where(x =>
                              x.Paths.Contains(item.ID) &&
                              x.TemplateId == blog.BlogSettings.CommentTemplateID &&
                              x.DatabaseName.Equals(item.Database.Name, StringComparison.InvariantCulture) &&
                              x.Language == item.Language.Name
                              );
                            if (indexresults.Any())
                            {
                                if (sort)
                                {
                                    if (reverse)
                                        indexresults = indexresults.OrderByDescending(x => x.FullCreatedDate);
                                    else
                                        indexresults = indexresults.OrderBy(x => x.FullCreatedDate);
                                }

                                indexresults = indexresults.Take(maximumCount);

                              // Had some odd issues with the linq layer. Array to avoid them.
                              var indexresultsList = indexresults.ToArray();

                              var items = from resultItem in indexresultsList
                                let i = resultItem.GetItem()
                                where i != null
                                select i;

                              result = items.Select(x => new CommentItem(x)).ToList();
                            }
                        }
                    }
                    return result.ToArray();
                }
            }
            return new CommentItem[0];
        }
    }
}
