using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.SecurityModel;
using Sitecore.Sites;
using Sitecore.StringExtensions;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
	public class CreateCommentItem : ICreateCommentProcessor
	{
        /// <summary>
        /// Gets or sets the <see cref="IBlogManager"/> used to access the structure of the blog and other settings.
        /// </summary>
	    protected IBlogManager BlogManager { get; set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
	    public CreateCommentItem()
	        : this(null)
	    {
	    }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="blogManager">The <see cref="IBlogManager"/> used to access the structure of the blog and other settings.</param>
	    public CreateCommentItem(IBlogManager blogManager)
        {
            BlogManager = blogManager ?? ManagerFactory.BlogManagerInstance;
        }

        public void Process(CreateCommentArgs args)
		{
            Assert.ArgumentNotNull(args, "args cannot be null");
			Assert.IsNotNull(args.Database, "Database cannot be null");
			Assert.IsNotNull(args.Comment, "Comment cannot be null");
			Assert.IsNotNull(args.EntryID, "Entry ID cannot be null");
            Assert.IsNotNull(args.Language, "Language cannot be null");

			var entryItem = args.Database.GetItem(args.EntryID, args.Language);
			if (entryItem != null)
			{
				var blogItem = BlogManager.GetCurrentBlog(entryItem);
				if (blogItem != null)
				{
					var template = args.Database.GetTemplate(blogItem.BlogSettings.CommentTemplateID);
					var itemName = ItemUtil.ProposeValidItemName(string.Format("Comment at {0} by {1}", GetDateTime().ToString("yyyyMMdd HHmmss"), args.Comment.AuthorName));
                    if (itemName.Length > 100)
                    {
                        itemName = itemName.Substring(0, 100);
                    }

					// verify the comment item name is unique for this entry
                    var query = "fast:{0}//{1}".FormatWith(ContentHelper.EscapePath(entryItem.Paths.FullPath), itemName);

					var num = 1;
					var nondupItemName = itemName;
					while (entryItem.Database.SelectSingleItem(query) != null)
					{
						nondupItemName = itemName + " " + num;
						num++;
						query = "fast:{0}//{1}".FormatWith(entryItem.Paths.FullPath, nondupItemName);
					}

					//need to emulate creation within shell site to ensure workflow is applied to comment
					using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext(Sitecore.Constants.ShellSiteName)))
					{
						using (new SecurityDisabler())
						{
							var newItem = entryItem.Add(nondupItemName, template);

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
					var message = "Failed to find blog for entry {0}\r\nIgnoring comment: name='{1}', email='{2}', commentText='{3}'";
                    Logger.Error(string.Format(message, args.EntryID, args.Comment.AuthorName, args.Comment.AuthorEmail, args.Comment.Text), typeof(CreateCommentItem));
				}
			}
			else
			{
				var message = "Failed to find blog entry {0}\r\nIgnoring comment: name='{1}', email='{2}', commentText='{3}'";
                Logger.Error(string.Format(message, args.EntryID, args.Comment.AuthorName, args.Comment.AuthorEmail, args.Comment.Text), typeof(CreateCommentItem));
			}
		}

	    protected virtual DateTime GetDateTime()
	    {
	        return DateTime.Now;
	    }
	}
}