using System;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
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
	    protected IBlogManager BlogManager { get; }

        /// <summary>
        /// Gets the <see cref="IBlogSettingsResolver"/> used to resolve the settings for a given blog item.
        /// </summary>
        protected IBlogSettingsResolver BlogSettingsResolver { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
	    public CreateCommentItem()
            : this(null, null)
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="blogManager">The <see cref="IBlogManager"/> used to access the structure of the blog and other settings.</param>
        [Obsolete("Use ctor(IBlogManager, IBlogSettingsResolver) instead.")]
	    public CreateCommentItem(IBlogManager blogManager)
            : this(blogManager, null)
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="blogManager">The <see cref="IBlogManager"/> used to access the structure of the blog and other settings.</param>
        /// <param name="blogSettingsResolver">The <see cref="IBlogSettingsResolver"/> used to resolve the settings for a given blog item.</param>
	    public CreateCommentItem(IBlogManager blogManager, IBlogSettingsResolver blogSettingsResolver)
        {
            BlogManager = blogManager ?? ServiceLocator.ServiceProvider.GetRequiredService<IBlogManager>();
            BlogSettingsResolver = blogSettingsResolver ?? ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>();
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
                    var settings = BlogSettingsResolver.Resolve(blogItem);

                    var template = args.Database.GetTemplate(settings.CommentTemplateID);
                    var itemName =
                        ItemUtil.ProposeValidItemName(string.Format("Comment at {0} by {1}",
                            GetDateTime().ToString("yyyyMMdd HHmmss"), args.Comment.AuthorName));
                    if (itemName.Length > 100)
                    {
                        itemName = itemName.Substring(0, 100);
                    }

                    // verify the comment item name is unique for this entry
                    var query = BuildFastQuery(entryItem, itemName);

                    var num = 1;
                    var nondupItemName = itemName;
                    while (entryItem.Database.SelectSingleItem(query) != null)
                    {
                        nondupItemName = itemName + " " + num;
                        num++;
                        query = BuildFastQuery(entryItem, nondupItemName);
                    }

                    // need to create the comment within the shell site to ensure workflow is applied to comment
                    var shellSite = SiteContextFactory.GetSiteContext(Sitecore.Constants.ShellSiteName);
                    SiteContextSwitcher siteSwitcher = null;

                    try
                    {
                        if (shellSite != null)
                        {
                            siteSwitcher = new SiteContextSwitcher(shellSite);
                        }

                        using (new SecurityDisabler())
                        {
                            var newItem = entryItem.Add(nondupItemName, template);

                            var newComment = new CommentItem(newItem);
                            newComment.BeginEdit();
                            newComment.CommentorName.Field.Value = args.Comment.AuthorName;
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
                    finally
                    {
                        siteSwitcher?.Dispose();
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

        protected virtual string BuildFastQuery(Item parentItem, string itemName)
        {
            string path = $"{parentItem.Paths.FullPath}/{itemName}";
            string escapePath = ContentHelper.EscapePath(path);
            return $"fast:{escapePath}";
        }

        protected virtual DateTime GetDateTime()
        {
            return DateTime.Now;
        }
    }
}