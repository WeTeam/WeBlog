using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.StringExtensions;
using System;

namespace Sitecore.Modules.WeBlog.Pipelines.CreateComment
{
    public class DuplicateSubmissionGuard : ICreateCommentProcessor
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
        /// The TimeSpan to check for duplicate comments from the current time
        /// </summary>
        public TimeSpan TimeSpan { get; set; }

        public DuplicateSubmissionGuard()
            : this(null, null)
        {
        }

        public DuplicateSubmissionGuard(IBlogManager blogManager = null, IBlogSettingsResolver blogSettingsResolver = null)
        {
            BlogManager = blogManager ?? ServiceLocator.ServiceProvider.GetRequiredService<IBlogManager>();
            BlogSettingsResolver = blogSettingsResolver ?? ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>();
            TimeSpan = new TimeSpan(3, 0, 0, 0);
        }

        public void Process(CreateCommentArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.Database, "Database cannot be null");
            Assert.IsNotNull(args.Comment, "Comment cannot be null");
            Assert.IsNotNull(args.EntryID, "Entry ID cannot be null");
            Assert.IsNotNull(args.Language, "Language cannot be null");

            var entryItem = args.Database.GetItem(args.EntryID, args.Language);
            if (entryItem != null)
            {
                // End at end of today to make sure we cover comments from today properly
                var dateEnd = System.DateTime.UtcNow.Date.AddDays(1);
                var dateStart = dateEnd - TimeSpan;

                var blog = BlogManager.GetCurrentBlog(entryItem);
                if (blog != null)
                {
                    var settings = BlogSettingsResolver.Resolve(blog);

                    var query = "{0}//*[@@templateid='{1}' and @__created > '{2}' and @__created < '{3}']".FormatWith(
                        ContentHelper.EscapePath(entryItem.Paths.FullPath), settings.CommentTemplateID, DateUtil.ToIsoDate(dateStart), DateUtil.ToIsoDate(dateEnd));

                    var comments = args.Database.SelectItems(query);

                    foreach (var item in comments)
                    {
                        var languageItem = args.Database.GetItem(item.ID, args.Language);
                        var commentItem = (CommentItem)languageItem;
                        if (string.Compare(commentItem.AuthorName, args.Comment.AuthorName, StringComparison.OrdinalIgnoreCase) == 0 &&
                            string.Compare(commentItem.Email.Raw, args.Comment.AuthorEmail, StringComparison.OrdinalIgnoreCase) == 0 &&
                            string.Compare(commentItem.Comment.Raw, args.Comment.Text, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            Logger.Warn("Duplicate comment submission. Existing item: {0}".FormatWith(commentItem.ID), this);
                            args.AbortPipeline();
                            return;
                        }
                    }
                }
            }
        }
    }
}