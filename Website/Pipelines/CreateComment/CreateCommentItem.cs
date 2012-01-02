using System;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class CreateCommentItem : ICreateCommentProcessor
    {
        public void Process(CreateCommentArgs args)
        {
            Assert.IsNotNull(args.Database, "Database cannot be null");
            Assert.IsNotNull(args.Comment, "Comment cannot be null");
            Assert.IsNotNull(args.EntryID, "Entry ID cannot be null");

            var template = args.Database.GetTemplate(Settings.CommentTemplateId);

            if (template != null)
            {
                var entryItem = args.Database.GetItem(args.EntryID);

                if (entryItem != null)
                {
                    string itemName = Utilities.Items.MakeSafeItemName("Comment by " + args.Comment.AuthorName + " at " + DateTime.Now.ToString("d"));

                    using (new SecurityDisabler())
                    {
                        var newItem = entryItem.Add(itemName, template);

                        var newComment = new CommentItem(newItem);
                        newComment.BeginEdit();
                        newComment.Name.Field.Value = args.Comment.AuthorName;
                        newComment.Email.Field.Value = args.Comment.AuthorEmail;
                        newComment.Comment.Field.Value = args.Comment.Text;

                        foreach (var key in args.Comment.Fields.AllKeys)
                        {
                            newComment.InnerItem[key] = args.Comment.Fields[key];
                        }

                        newComment.EndEdit();

                        args.CommentItem = newComment;
                    }
                }
                else
                {
                    string message = "WeBlog.CommentManager: Failed to find blog entry {0}\r\nIgnoring comment: name='{1}', email='{2}', commentText='{3}'";
                    Log.Error(string.Format(message, args.EntryID, args.Comment.AuthorName, args.Comment.AuthorEmail, args.Comment.Text), typeof(CreateCommentItem));
                }
            }
            else
                Log.Error("WeBlog.CommentManager: Failed to find comment template", typeof(CreateCommentItem));
        }
    }
}