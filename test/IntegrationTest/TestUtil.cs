using System;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Security.Accounts;

namespace Sitecore.Modules.WeBlog.IntegrationTest
{
    public static class TestUtil
    {
        public static BlogHomeItem CreateNewBlog(Item parentItem)
        {
            using (new UserSwitcher("sitecore\\admin", true))
            {
                var name = "blog " + ID.NewID.ToShortID();
                return parentItem.Add(name, Constants.Branches.BlogBranchId);
            }
        }

        public static EntryItem CreateNewEntry(BlogHomeItem blogItem, string name, string tags = null, ID[] categories = null, DateTime? entryDate = null)
        {
            using (new UserSwitcher("sitecore\\admin", true))
            {
                var entry = blogItem.InnerItem.Add(name, Constants.Templates.EntryTemplateId);

                if (tags != null)
                {
                    using (new EditContext(entry))
                    {
                        entry["Tags"] = tags;
                    }
                }

                if (categories != null)
                {
                    using (new EditContext(entry))
                    {
                        entry["Category"] = string.Join<ID>("|", categories);
                    }
                }

                if (entryDate != null)
                {
                    using (new EditContext(entry))
                    {
                        entry["Entry Date"] = DateUtil.ToIsoDate(entryDate.Value);
                    }
                }

                return entry;
            }
        }

        public static CategoryItem CreateNewCategory(BlogHomeItem blogItem, string name)
        {
            using (new UserSwitcher("sitecore\\admin", true))
            {
                var categoryRoot = blogItem.InnerItem.Children["Categories"];
                return categoryRoot.Add(name, Constants.Templates.CategoryTemplateId);
            }
        }

        public static CommentItem CreateNewComment(EntryItem entryItem, DateTime? creationDate = null)
        {
            if(creationDate == null)
                creationDate = DateTime.Now;

            using (new UserSwitcher("sitecore\\admin", true))
            {
                var commentItem = entryItem.InnerItem.Add(ID.NewID.ToShortID().ToString(), Constants.Templates.CommentTemplateId);
                using (new EditContext(commentItem))
                {
                    commentItem[FieldIDs.Created] = DateUtil.ToIsoDate(creationDate.Value);
                }

                return commentItem;
            }
        }

        public static void UpdateIndex()
        {
            var settings = WeBlogSettings.Instance;
            var index = ContentSearchManager.GetIndex(settings.SearchIndexName + "-master");
            index.Rebuild();
        }
    }
}