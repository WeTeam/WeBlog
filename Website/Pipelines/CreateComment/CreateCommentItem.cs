using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.SecurityModel;
using Sitecore.Sites;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class CreateCommentItem : ICreateCommentProcessor
    {
        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.Database, "Database cannot be null");
            Assert.IsNotNull(args.Comment, "Comment cannot be null");
            Assert.IsNotNull(args.EntryID, "Entry ID cannot be null");

            var entryItem = args.Database.GetItem(args.EntryID, args.Language);
            if (entryItem != null)
            {
                var blogItem = ManagerFactory.BlogManagerInstance.GetCurrentBlog(entryItem);
                if (blogItem != null)
                {
                    var template = args.Database.GetTemplate(blogItem.BlogSettings.CommentTemplateID);
                    string itemName = ItemUtil.ProposeValidItemName("Comment by " + args.Comment.AuthorName + " at " + DateTime.Now.ToString("d"));

                    //need to emulate creation within shell site to ensure workflow is applied to comment
                    using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext("shell")))
                    {
                        using (new SecurityDisabler())
                        {
                            var newItem = entryItem.Add(itemName, template);

                            var newComment = new CommentItem(newItem);
                            newComment.BeginEdit();
                            newComment.Name.Field.Value = args.Comment.AuthorName;
                            newComment.Email.Field.Value = args.Comment.AuthorEmail;
                            newComment.Comment.Field.Value = args.Comment.Text;

                            foreach (var entry in args.Comment.Fields)
                            {
                                newComment.InnerItem[entry.Key] = entry.Value;
                            }

                            newComment.EndEdit();

                            args.CommentItem = newComment;
                        }
                    }
                }
                else
                {
                    string message = "Failed to find blog for entry {0}\r\nIgnoring comment: name='{1}', email='{2}', commentText='{3}'";
                    Log.Error(string.Format(message, args.EntryID, args.Comment.AuthorName, args.Comment.AuthorEmail, args.Comment.Text), typeof(CreateCommentItem));
                }
            }
            else
            {
                string message = "Failed to find blog entry {0}\r\nIgnoring comment: name='{1}', email='{2}', commentText='{3}'";
                Log.Error(string.Format(message, args.EntryID, args.Comment.AuthorName, args.Comment.AuthorEmail, args.Comment.Text), typeof(CreateCommentItem));
            }
        }
    }
}