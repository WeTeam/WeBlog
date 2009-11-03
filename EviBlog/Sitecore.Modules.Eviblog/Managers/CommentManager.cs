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

namespace Sitecore.Modules.Eviblog.Managers
{
    public class CommentManager
    {
        public static void AddCommentToEntry(string Name, string Email, string Website, string CommentText)
        {
            SecurityDisabler securitydisabler = new SecurityDisabler();
            // Get database
            Database database = Factory.GetDatabase("master");
            
            // Create item
            TemplateItem template = database.GetTemplate(Settings.Default.CommentTemplateID);
            string itemName = "Comment by " + Name + " at " + DateTime.Now.ToString("MM-dd-yyyy");
            Item currentItem = Context.Item;
            Item currentMasterItem = database.GetItem(currentItem.ID);

            // Create comment item
            if (currentMasterItem != null)
            {
                Item newItem = currentMasterItem.Add(itemName, template);

                Comment newComment = new Comment(newItem);
                newComment.BeginEdit();
                newComment.UserName = Name;
                newComment.Email = Email;
                newComment.Website = Website;
                newComment.CommentText = CommentText;
                newComment.EndEdit();
                SecurityEnabler securityenabler = new SecurityEnabler();

                Publish.PublishItem(newItem);

                WebUtil.ReloadPage();
            }
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
