using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Comparers;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Import;
using Sitecore.Modules.WeBlog.Items.WeBlog;
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
        /// Adds a comment to a blog
        /// </summary>
        /// <param name="entryId">The ID of the entry to add the comment to</param>
        /// <param name="comment">The comment to add to the entry</param>
        /// <param name="language">The language to create the comment in</param>
        /// <returns>The ID of the created comment item, or ID.Null if creation failed</returns>
        public ID AddCommentToEntry(ID entryId, Model.Comment comment, Language language = null)
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

        protected void AddComment(EntryItem entryItem, WpComment wpComment)
        {
            // todo: Wizard to ask user which language to import into
            string itemName = ItemUtil.ProposeValidItemName("Comment by " + wpComment.Author + " at " + wpComment.Date.ToString("d"));

            CommentItem commentItem = entryItem.InnerItem.Add(itemName, new TemplateID(new ID(CommentItem.TemplateId)));
            commentItem.BeginEdit();
            commentItem.Comment.Field.Value = wpComment.Content;
            commentItem.Email.Field.Value = wpComment.Email;
            commentItem.IPAddress.Field.Value = wpComment.IP;
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
        public ID SubmitComment(ID entryId, Model.Comment comment, Language language = null)
        {
            if (Configuration.Settings.GetBoolSetting("WeBlog.CommentService.Enable", false))
            {
                // Submit comment through WCF service
                ChannelFactory<ICommentService> commentProxy = new ChannelFactory<ICommentService>("WeBlogCommentService");
                commentProxy.Open();
                ICommentService service = commentProxy.CreateChannel();
                var result = service.SubmitComment(Context.Item.ID, comment, language);
                commentProxy.Close();
                if (result == ID.Null)
                    Log.Error("WeBlog.CommentManager: Comment submission through WCF failed. Check server Sitecore log for details", typeof(CommentManager));
                return result;
            }
            else
                return AddCommentToEntry(Context.Item.ID, comment, language);
        }

        /// <summary>
        /// Gets the number of comments for the current entry.
        /// </summary>
        /// <param name="language">The language to check comments in</param>
        /// <returns>The number of comments</returns>
        public int GetCommentsCount(Language language = null)
        {
            var item = Context.Item;
            if (language != null)
                item = item.Database.GetItem(item.ID, language);

            return GetCommentsCount(item);
        }

        /// <summary>
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entry">The entry to get the comment count for</param>
        /// <returns>The number of comments</returns>
        public int GetCommentsCount(Item entry)
        {
            return GetEntryComments(entry).Length;
        }

        /// <summary>
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entryId">The ID of the entry to get the comment count for</param>
        /// <param name="language">The language to check comments in</param>
        /// <returns>The number of comments</returns>
        public int GetCommentsCount(ID entryId, Language language = null)
        {
            Item entry = null;

            if (language != null)
                entry = GetDatabase().GetItem(entryId, language);
            else
                entry = GetDatabase().GetItem(entryId);

            if (entry != null)
                return GetCommentsCount(entry);
            else
                return 0;
        }

        /// <summary>
        /// Gets the comments for the blog entry
        /// </summary>
        /// <param name="blogId">The ID of the blog to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="language">The language to get the comments in</param>
        /// <returns>The comments for the blog entry</returns>
        public CommentItem[] GetCommentsByBlog(ID blogId, int maximumCount, Language language = null)
        {
            if (blogId != (ID)null)
            {
                Item blogItem = null;

                if (language != null)
                    blogItem = GetDatabase().GetItem(blogId, language);
                else
                    blogItem = GetDatabase().GetItem(blogId);

                if (blogItem != null)
                {
                    return GetCommentsByBlog(blogItem, maximumCount);
                }
            }

            return new CommentItem[0];
        }

        /// <summary>
        /// Gets the comments for the blog entry
        /// </summary>
        /// <param name="blogItem">The blog to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        public CommentItem[] GetCommentsByBlog(Item blogItem, int maximumCount)
        {
            return GetCommentsFor(blogItem, maximumCount, true, true);
        }

        /// <summary>
        /// Gets the comments for the current blog entry
        /// </summary>
        /// <param name="language">The language to get comments in</param>
        /// <returns>The comments for the blog entry</returns>
        public CommentItem[] GetEntryComments(Language language = null)
        {
            return GetEntryComments(int.MaxValue, language);
        }

        /// <summary>
        /// Gets the comments for the current blog entry
        /// </summary>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="language">The language to get comments in</param>
        /// <returns>The comments for the blog entry</returns>
        public CommentItem[] GetEntryComments(int maximumCount, Language language = null)
        {
            var blogItem = Context.Item;
            if (language != null && blogItem.Language != language && blogItem.Languages.Contains(language))
                blogItem = blogItem.Database.GetItem(blogItem.ID, language);
            return GetEntryComments(blogItem, maximumCount);
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <returns>The comments for the blog entry</returns>
        public CommentItem[] GetEntryComments(Item entryItem)
        {
            return GetEntryComments(entryItem, int.MaxValue);
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        public CommentItem[] GetEntryComments(Item entryItem, int maximumCount)
        {
            if (entryItem != null)
            {
                var template = GetDatabase().GetTemplate(Settings.EntryTemplateIDString);
                if (entryItem.TemplateIsOrBasedOn(template))
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
        public CommentItem[] GetCommentsFor(Item item, int maximumCount, bool sort = false, bool reverse = false)
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
                        using (var context = ContentSearchManager.GetIndex(indexName).CreateSearchContext())
                        {
                            var indexresults = context.GetQueryable<CommentResultItem>().Where(x => 
                              x.Paths.Contains(item.ID) &&
                              x.TemplateId == blog.BlogSettings.CommentTemplateID &&
                              x.DatabaseName.Equals(Context.Database.Name, StringComparison.InvariantCulture) &&
                              x.Language == item.Language.Name
                              );
                            if (indexresults.Any())
                            {
                              if(sort)
                                indexresults = indexresults.OrderByDescending(x => x.CreatedDate);
                                
                              else if (reverse)
                                indexresults = indexresults.OrderBy(x => x.CreatedDate);

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

        /// <summary>
        /// Gets the appropriate database to be reading data from
        /// </summary>
        /// <returns>The appropriate content database</returns>
        protected Database GetDatabase()
        {
            return Context.ContentDatabase ?? Context.Database;
        }

        #region Obsolete Methods
        /// <summary>
        /// Gets the comments for the given blog entry as Items
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <returns>The comments for the blog entry</returns>
        [Obsolete("Use GetEntryComments(Item entryItem).InnerItem instead")]
        public static Item[] GetEntryCommentsAsItems(Item targetEntry)
        {
            return (from comment in new CommentManager().GetEntryComments(targetEntry) select comment.InnerItem).ToArray();
        }

        /// <summary>
        /// Gets the comments for the blog entry as items
        /// </summary>
        /// <param name="blogId">The ID of the blog to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        [Obsolete("Use GetCommentsByBlog(ID blogId, int maximumCount).InnerItem instead")]
        public static Item[] GetCommentItemsByBlog(ID blogId, int maximumCount)
        {
            return (from comment in new CommentManager().GetCommentsByBlog(blogId, maximumCount) select comment.InnerItem).ToArray();
        }

        /// <summary>
        /// Makes the sorted comment item list.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        [Obsolete("Use InnerItem of comment object")]
        public static Item[] MakeSortedCommentsListAsItems(System.Collections.IList array)
        {
            var commentItemList = new List<Item>();
            foreach (Item item in array)
            {
                if (item.TemplateID == Settings.CommentTemplateID && item.Versions.GetVersions().Length > 0)
                {
                    commentItemList.Add(item);
                }
            }
            commentItemList.Sort(new ItemDateComparerDesc());
            return commentItemList.ToArray();
        }

        /// <summary>
        /// Sort the comments list using the CommentDateComparerDesc comparer
        /// </summary>
        /// <param name="array">The comments to sort</param>
        /// <returns>A sorted list of comments</returns>
        [Obsolete("Use sorting options on GetCommentsFor")]
        public CommentItem[] MakeSortedCommentsList(System.Collections.IList array)
        {
            var commentItemList = new List<CommentItem>();
            var template = GetDatabase().GetTemplate(Settings.CommentTemplateID);

            if (template != null)
            {
                foreach (Item item in array)
                {
                    if (item.TemplateIsOrBasedOn(template) && item.Versions.Count > 0)
                    {
                        commentItemList.Add(new CommentItem(item));
                    }
                }

                commentItemList.Sort(new CommentDateComparerDesc());
            }

            return commentItemList.ToArray();
        }
        #endregion
    }
}
