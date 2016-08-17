using System;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.Test
{
    public static class TestUtil
    {
        public static Item CreateContentFromFile(string filename, Item parent, bool changeIds = true)
        {
            var xml = File.ReadAllText(HttpContext.Current.Server.MapPath(filename));
            if (string.IsNullOrEmpty(xml))
                return null;

            return parent.PasteItem(xml, changeIds, PasteMode.Merge);
        }

        public static bool IsGermanRegistered(Database database)
        {
            return (from l in database.Languages
                    where l.Name == "de"
                    select l).Any();
        }

        public static Item RegisterGermanLanaguage(Database database)
        {
            using (new SecurityDisabler())
            {
                var languageRoot = database.GetItem(ItemIDs.LanguageRoot);
                return TestUtil.CreateContentFromFile("test data\\German Language.xml", languageRoot);
            }
        }

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
            var settings = new WeBlogSettings();
            var index = ContentSearchManager.GetIndex(settings.SearchIndexName);
            index.Rebuild();
        }
    }
}