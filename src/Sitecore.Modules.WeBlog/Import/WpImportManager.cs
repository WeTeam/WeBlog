using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using System;
using Sitecore.Data.Templates;
using Sitecore.Modules.WeBlog.Import.Providers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.Import
{
    public class WpImportManager
    {
        private readonly Database _db;
        private readonly IWpPostProvider _wpPostProvider;
        private readonly WpImportOptions _options;
        private List<WpPost> _posts;

        public List<WpPost> Posts
        {
            get { return _posts ?? (_posts = _wpPostProvider.GetPosts(_options)); }
        }

        public WpImportManager(Database db, IWpPostProvider wpPostProvider, WpImportOptions options)
        {
            _db = db;
            _wpPostProvider = wpPostProvider;
            _options = options;
        }

        public BlogHomeItem CreateBlogRoot(Item root, string name, string email)
        {
            BranchItem newBlog = _db.Branches.GetMaster(Settings.BlogBranchID);
            BlogHomeItem blogItem = root.Add(ItemUtil.ProposeValidItemName(name), newBlog);

            blogItem.BeginEdit();
            blogItem.Email.Field.Value = email;
            blogItem.EndEdit();

            return blogItem;
        }

        internal ImportSummary ImportPosts(Item blogItem, TemplatesMapping mapping, Action<string, int> logger = null)
        {
            var summary = new ImportSummary();

            var entryTemplate = TemplateManager.GetTemplate(mapping.BlogEntryTemplate, _db);

            foreach (WpPost post in Posts)
            {
                summary.PostCount++;

                if (!string.IsNullOrEmpty(post.Content))
                {
                    var title = post.Title;
                    title = String.IsNullOrEmpty(title) ? $"Post {Posts.IndexOf(post)}" : title;
                    var name = ItemUtil.ProposeValidItemName(title);

                    if (logger != null)
                        logger(name, summary.PostCount);

                    EntryItem entry = ItemManager.AddFromTemplate(name, entryTemplate.ID, blogItem);

                    entry.BeginEdit();
                    entry.Title.Field.Value = post.Title;
                    entry.Introduction.Field.Value = string.Empty;
                    entry.Content.Field.Value = post.Content;
                    entry.Tags.Field.Value = string.Join(", ", post.Tags.ToArray());

                    var categorieItems = new List<string>();

                    foreach (string categoryName in post.Categories)
                    {
                        var categoryItem = ManagerFactory.CategoryManagerInstance.Add(categoryName, blogItem);
                        categoryItem.InnerItem.ChangeTemplate(categoryItem.Database.GetItem(mapping.BlogCategoryTemplate));
                        categorieItems.Add(categoryItem.ID.ToString());
                        summary.CategoryCount++;
                    }

                    if (categorieItems.Count > 0)
                    {
                        entry.Category.Field.Value = string.Join("|", categorieItems.ToArray());
                    }

                    foreach (WpComment wpComment in post.Comments)
                    {
                        var commentId = ManagerFactory.CommentManagerInstance.AddCommentToEntry(entry.ID, wpComment);
                        var comment = entry.Database.GetItem(commentId);
                        comment?.ChangeTemplate(comment.Database.GetItem(mapping.BlogCommentTemplate));

                        summary.CommentCount++;
                    }

                    var publicationDate = DateUtil.ToIsoDate(post.PublicationDate);
                    entry.InnerItem.Fields[FieldIDs.Created].Value = publicationDate;
                    entry.InnerItem.Fields["Entry Date"].Value = publicationDate;
                    entry.EndEdit();
                }
            }

            return summary;
        }
    }
}