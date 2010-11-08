using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.Eviblog.Comparers;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.SecurityModel;
using Sitecore.Modules.Eviblog.Utilities;
using Sitecore.Web;
using Sitecore.Modules.Eviblog.Services;
using System.ServiceModel;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.Eviblog.Managers
{
    public class CommentManager
    {
        public static bool AddCommentToEntry(ID EntryId, Model.Comment comment)
        {
            using (new SecurityDisabler())
            {
                // Get database
                Database database = Factory.GetDatabase("master");

                // Create item
                TemplateItem template = database.GetTemplate(Settings.Default.CommentTemplateID);
                string itemName = Utilities.Items.MakeSafeItemName("Comment by " + comment.AuthorName + " at " + DateTime.Now.ToString("d"));

                Item currentMasterItem = database.GetItem(EntryId);

                // Create comment item
                if (currentMasterItem != null)
                {
                    Item newItem = currentMasterItem.Add(itemName, template);

                    Comment newComment = new Comment(newItem);
                    newComment.BeginEdit();
                    newComment.UserName = comment.AuthorName;
                    newComment.Email = comment.AuthorEmail;
                    newComment.Website = comment.AuthorWebsite;
                    newComment.IPAddress = comment.AuthorIP;
                    newComment.CommentText = comment.Text;
                    newComment.EndEdit();

                    Publish.PublishItem(newItem);

                    WebUtil.ReloadPage();
                }
                else
                {
                    string message = "EviBlog.CommentManager: Failed to find blog entry {0}\r\nIgnoring comment: name='{1}', email='{2}', website='{3}', commentText='{4}', IP='{5}'";
                    Log.Error(string.Format(message, EntryId, comment.AuthorName, comment.AuthorEmail, comment.AuthorWebsite, comment.Text, comment.AuthorIP), typeof(CommentManager));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Submit a comment for inclusion on a post. This method will either update Sitecore or submit the comment through the comment service, depending on settings
        /// </summary>
        /// <param name="Name">The name of the user submitting the comment</param>
        /// <param name="Email">The user's email address</param>
        /// <param name="Website">The user's URL</param>
        /// <param name="CommentText">The comment text being submitted</param>
        public static bool SubmitComment(ID EntryId, Model.Comment comment)
        {
            if (Configuration.Settings.GetBoolSetting("EviBlog.CommentService.Enable", false))
            {
                // Submit comment through WCF service
                ChannelFactory<ICommentService> commentProxy = new ChannelFactory<ICommentService>("EviBlogCommentService");
                commentProxy.Open();
                ICommentService service = commentProxy.CreateChannel();
                bool result = service.SubmitComment(Context.Item.ID, comment);
                commentProxy.Close();
                if (!result)
                    Log.Error("EviBlog.CommentManager: Comment submission through WCF failed. Check server Sitecore log for details", typeof(CommentManager));
                return result;
            }
            else
                return AddCommentToEntry(Context.Item.ID, comment);
        }

        /// <summary>
        /// Gets the number of comments under the current entry.
        /// </summary>
        /// <returns></returns>
        public static int GetCommentsCount()
        {
            return Context.Item.Children.Count;
        }

        /// <summary>
        /// Gets the comments count.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <returns></returns>
        public static int GetCommentsCount(ID currentItem)
        {
            Item currentEntry = Context.Database.GetItem(currentItem);

            return currentEntry.Children.Count;
        }

        /// <summary>
        /// Gets the entry comments.
        /// </summary>
        /// <returns></returns>
        public static List<Comment> GetCommentsByBlog(ID BlogID, int Total)
        {
            List<Comment> CommentList = new List<Comment>();
            
            int count = 0;

            Item currentBlog = Context.Database.GetItem(BlogID);

            foreach (Item itm in currentBlog.Children)
            {
                if (itm.TemplateID.ToString() == Settings.Default.EntryTemplateID)
                {
                    foreach (Comment entry in MakeSortedCommentsList(itm.Children.ToArray()))
                    {
                        if (count < Total)
                        {
                            CommentList.Add(entry);
                            count++;
                        }
                    }
                }
            }

            return CommentList;
        }

        /// <summary>
        /// Gets the entry comments.
        /// </summary>
        /// <returns></returns>
        public static List<Item> GetCommentItemsByBlog(ID BlogID, int Total)
        {
            List<Item> CommentList = new List<Item>();

            int count = 0;

            Item currentBlog = Context.Database.GetItem(BlogID);

            foreach (Item itm in currentBlog.Children)
            {
                if (itm.TemplateID.ToString() == Settings.Default.EntryTemplateID)
                {
                    foreach (Item entry in MakeSortedCommentsListAsItems(itm.Children.ToArray()))
                    {
                        if (count < Total)
                        {
                            CommentList.Add(entry);
                            count++;
                        }
                    }
                }
            }

            return CommentList;
        }

        /// <summary>
        /// Gets the entry comments.
        /// </summary>
        /// <returns></returns>
        public static List<Comment> GetEntryComments()
        {
            List<Comment> CommentList = new List<Comment>();

            Item current = Context.Item;

            foreach (Comment entry in MakeSortedCommentsList(current.Children.ToArray()))
            {
                    CommentList.Add(entry);
            }

            return CommentList;
        }

        /// <summary>
        /// Gets the entry comments.
        /// </summary>
        /// <param name="NumberOfComments">The number of comments.</param>
        /// <returns></returns>
        public static List<Comment> GetEntryComments(int NumberOfComments)
        {
            List<Comment> CommentList = new List<Comment>();
            int count = 0;
            Item current = Context.Item;

            foreach (Comment entry in MakeSortedCommentsList(current.Children.ToArray()))
            {
                if (NumberOfComments < count)
                {
                    CommentList.Add(entry);
                    count++;
                }
            }

            return CommentList;
        }

        /// <summary>
        /// Gets the entry comments.
        /// </summary>
        /// <param name="targetEntry">The target entry.</param>
        /// <returns></returns>
        public static List<Comment> GetEntryComments(Item targetEntry)
        {
            List<Comment> CommentList = new List<Comment>();

            foreach (Comment entry in MakeSortedCommentsList(targetEntry.Children.ToArray()))
            {
                CommentList.Add(entry);
            }

            return CommentList;
        }
        /// Gets the entry comments.
        /// </summary>
        /// <param name="targetEntry">The target entry.</param>
        /// <returns></returns>
        public static List<Item> GetEntryCommentsAsItems(Item targetEntry)
        {
            List<Item> CommentList = new List<Item>();

            foreach (Item entry in MakeSortedCommentsListAsItems(targetEntry.Children.ToArray()))
            {
                CommentList.Add(entry);
            }

            return CommentList;
        }
        /// <summary>
        /// Gets the entry comments.
        /// </summary>
        /// <param name="targetEntry">The target entry.</param>
        /// <param name="NumberOfComments">The number of comments.</param>
        /// <returns></returns>
        public static List<Comment> GetEntryComments(Item targetEntry, int NumberOfComments)
        {
            List<Comment> CommentList = new List<Comment>();
            int count = 0;

            foreach (Comment entry in MakeSortedCommentsList(targetEntry.Children.ToArray()))
            {
                if (NumberOfComments < count)
                {
                    CommentList.Add(entry);
                    count++;
                }
            }

            return CommentList;
        }

        /// <summary>
        /// Makes the sorted comment item list.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static List<Comment> MakeSortedCommentsList(System.Collections.IList array)
        {
            List<Comment> commentItemList = new List<Comment>();
            foreach (Item item in array)
            {
                if (item.TemplateID.ToString() == Settings.Default.CommentTemplateID && item.Versions.GetVersions().Length > 0)
                {
                    commentItemList.Add(new Comment(item));
                }
            }
            commentItemList.Sort(new CommentDateComparerDesc());
            return commentItemList;
        }
        /// <summary>
        /// Makes the sorted comment item list.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static List<Item> MakeSortedCommentsListAsItems(System.Collections.IList array)
        {
            List<Item> commentItemList = new List<Item>();
            foreach (Item item in array)
            {
                if (item.TemplateID.ToString() == Settings.Default.CommentTemplateID && item.Versions.GetVersions().Length > 0)
                {
                    commentItemList.Add(item);
                }
            }
            commentItemList.Sort(new ItemDateComparerDesc());
            return commentItemList;
        }
    }
}
