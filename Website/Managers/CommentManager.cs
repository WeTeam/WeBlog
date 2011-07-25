using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Comparers;
using Sitecore.Modules.WeBlog.Items.Blog;
using Sitecore.Modules.WeBlog.Services;
using Sitecore.Modules.WeBlog.Utilities;
using Sitecore.SecurityModel;
using Sitecore.Web;
using Sitecore.Modules.WeBlog.Import;

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with comments
    /// </summary>
    public static class CommentManager
    {
        /// <summary>
        /// Adds a comment to a blog
        /// </summary>
        /// <param name="entryId">The ID of the entry to add the comment to</param>
        /// <param name="comment">The comment to add to the entry</param>
        /// <returns>The ID of the created comment item, or ID.Null if creation failed</returns>
        public static ID AddCommentToEntry(ID entryId, Model.Comment comment)
        {
            var database = Factory.GetDatabase("master");
            var template = database.GetTemplate(Sitecore.Configuration.Settings.GetSetting("Blog.CommentTemplateID"));

            if (template != null)
            {
                var entryItem = database.GetItem(entryId);

                if (entryItem != null)
                {
                    string itemName = Utilities.Items.MakeSafeItemName("Comment by " + comment.AuthorName + " at " + DateTime.Now.ToString("d"));

                    using (new SecurityDisabler())
                    {
                        Item newItem = entryItem.Add(itemName, template);

                        CommentItem newComment = new CommentItem(newItem);
                        newComment.BeginEdit();
                        newComment.Name.Field.Value = comment.AuthorName;
                        newComment.Email.Field.Value = comment.AuthorEmail;
                        newComment.Website.Field.Value = comment.AuthorWebsite;
                        newComment.IPAddress.Field.Value = comment.AuthorIP;
                        newComment.Comment.Field.Value = comment.Text;
                        newComment.EndEdit();

                        Publish.PublishItem(newItem);

                        // Ensure current request is not from the test page (Context.Item is not null)
                        if(Sitecore.Context.Item != null)
                            WebUtil.ReloadPage();

                        return newComment.ID;
                    }
                }
                else
                {
                    string message = "WeBlog.CommentManager: Failed to find blog entry {0}\r\nIgnoring comment: name='{1}', email='{2}', website='{3}', commentText='{4}', IP='{5}'";
                    Log.Error(string.Format(message, entryId, comment.AuthorName, comment.AuthorEmail, comment.AuthorWebsite, comment.Text, comment.AuthorIP), typeof(CommentManager));
                }
            }
            else
            {
                Log.Error("WeBlog.CommentManager: Failed to find comment template", typeof(CommentManager));
            }
            
            // Something went wrong if we fall through to here
            return ID.Null;
        }

        internal static void AddComment(EntryItem entryItem, WpComment wpComment)
        {
            string itemName = Utilities.Items.MakeSafeItemName("Comment by " + wpComment.Author + " at " + wpComment.Date.ToString("d"));

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
        /// <param name="Name">The name of the user submitting the comment</param>
        /// <param name="Email">The user's email address</param>
        /// <param name="Website">The user's URL</param>
        /// <param name="CommentText">The comment text being submitted</param>
        public static ID SubmitComment(ID EntryId, Model.Comment comment)
        {
            if (Configuration.Settings.GetBoolSetting("WeBlog.CommentService.Enable", false))
            {
                // Submit comment through WCF service
                ChannelFactory<ICommentService> commentProxy = new ChannelFactory<ICommentService>("WeBlogCommentService");
                commentProxy.Open();
                ICommentService service = commentProxy.CreateChannel();
                var result = service.SubmitComment(Context.Item.ID, comment);
                commentProxy.Close();
                if (result == ID.Null)
                    Log.Error("WeBlog.CommentManager: Comment submission through WCF failed. Check server Sitecore log for details", typeof(CommentManager));
                return result;
            }
            else
                return AddCommentToEntry(Context.Item.ID, comment);
        }

        /// <summary>
        /// Gets the number of comments for the current entry.
        /// </summary>
        /// <returns>The number of comments</returns>
        public static int GetCommentsCount()
        {
            return GetCommentsCount(Context.Item);
        }

        /// <summary>
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entry">The entry to get the comment count for</param>
        /// <returns>The number of comments</returns>
        public static int GetCommentsCount(Item entry)
        {
            var template = GetDatabase().GetTemplate(Sitecore.Configuration.Settings.GetSetting("Blog.CommentTemplateID"));
            var count = 0;

            if (entry != null && template != null)
            {
                var descendants = entry.Axes.GetDescendants();
                foreach (Item descendant in descendants)
                {
                    if (Utilities.Items.TemplateIsOrBasedOn(descendant, template))
                        count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entryId">The ID of the entry to get the comment count for</param>
        /// <returns></returns>
        public static int GetCommentsCount(ID entryId)
        {
            var entry = GetDatabase().GetItem(entryId);
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
        /// <returns>The comments for the blog entry</returns>
        public static CommentItem[] GetCommentsByBlog(ID blogId, int maximumCount)
        {
            if (blogId != (ID)null)
            {
                var blogItem = GetDatabase().GetItem(blogId);
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
        public static CommentItem[] GetCommentsByBlog(Item blogItem, int maximumCount)
        {
            var commentList = new List<CommentItem>();

            //refactored to run as sitecore query. inefficient but gets most recent from ALL comments
            //could be refactored again to use Lucene?
            if (blogItem != null)
            {
                var commentTemplateID = Sitecore.Configuration.Settings.GetSetting("Blog.CommentTemplateID");
                var comments = blogItem.Axes.SelectItems(string.Format(".//*[@@templateid='{0}']", commentTemplateID));
                if (comments != null)
                {
                    commentList = MakeSortedCommentsList(comments).ToList();
                    //commentList.Reverse();
                    return commentList.Take(maximumCount).ToArray();
                }
                else
                {
                    Log.Error("CommentManager.GetCommentsByBlog: Failed to find blog entry template", typeof(CommentManager));
                }
            }
            return commentList.ToArray();
        }

        /// <summary>
        /// Gets the comments for the current blog entry
        /// </summary>
        /// <returns>The comments for the blog entry</returns>
        public static CommentItem[] GetEntryComments()
        {
            return GetEntryComments(Context.Item, int.MaxValue);
        }

        /// <summary>
        /// Gets the comments for the current blog entry
        /// </summary>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        public static CommentItem[] GetEntryComments(int maximumCount)
        {
            return GetEntryComments(Context.Item, maximumCount);
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <returns>The comments for the blog entry</returns>
        public static CommentItem[] GetEntryComments(Item entryItem)
        {
            return GetEntryComments(entryItem, int.MaxValue);
        }

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        public static CommentItem[] GetEntryComments(Item entryItem, int maximumCount)
        {
            var comments = new List<CommentItem>();

            if (entryItem != null)
            {
                var template = GetDatabase().GetTemplate(Sitecore.Configuration.Settings.GetSetting("Blog.EntryTemplateID"));
                if (Utilities.Items.TemplateIsOrBasedOn(entryItem, template))
                {
                    var count = 0;
                    foreach (var comment in MakeSortedCommentsList(entryItem.Axes.GetDescendants()))
                    {
                        if (count < maximumCount)
                        {
                            comments.Add(comment);
                            count++;
                        }
                    }
                }
            }

            return comments.ToArray();
        }

        /// <summary>
        /// Sort the comments list using the CommentDateComparerDesc comparer
        /// </summary>
        /// <param name="array">The comments to sort</param>
        /// <returns>A sorted list of comments</returns>
        public static CommentItem[] MakeSortedCommentsList(System.Collections.IList array)
        {
            var commentItemList = new List<CommentItem>();
            var template = GetDatabase().GetTemplate(Sitecore.Configuration.Settings.GetSetting("Blog.CommentTemplateID"));

            if (template != null)
            {
                foreach (Item item in array)
                {
                    if (Utilities.Items.TemplateIsOrBasedOn(item, template) && item.Versions.Count > 0)
                    {
                        commentItemList.Add(new CommentItem(item));
                    }
                }

                commentItemList.Sort(new CommentDateComparerDesc());
            }

            return commentItemList.ToArray();
        }

        /// <summary>
        /// Gets the appropriate database to be reading data from
        /// </summary>
        /// <returns>The appropriate content database</returns>
        private static Database GetDatabase()
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
            return (from comment in GetEntryComments(targetEntry) select comment.InnerItem).ToArray();
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
            return (from comment in GetCommentsByBlog(blogId, maximumCount) select comment.InnerItem).ToArray();
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
                if (item.TemplateID.ToString() == Sitecore.Configuration.Settings.GetSetting("Blog.CommentTemplateID") && item.Versions.GetVersions().Length > 0)
                {
                    commentItemList.Add(item);
                }
            }
            commentItemList.Sort(new ItemDateComparerDesc());
            return commentItemList.ToArray();
        }
        #endregion
    }
}
