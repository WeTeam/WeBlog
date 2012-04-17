using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Data;

namespace Sitecore.Modules.WeBlog.Test.CustomBlog
{
    public static class Extensions
    {
        public static void SetupCustomBlogs(this Database database, string testRoot)
        {
            var templateRoot = database.GetItem("/sitecore/templates/user defined");
            using (new SecurityDisabler())
            {
                templateRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\custom blog templates.xml")), false, PasteMode.Overwrite);
            }

            var home = database.GetItem("/sitecore/content/home");
            var testRootItem = home.Axes.GetChild(testRoot);
            var blogTemplate = Sitecore.Context.Database.GetTemplate("user defined/test templates/CustomBlog");
            var entryTemplate = Sitecore.Context.Database.GetTemplate("user defined/test templates/CustomEntry");
            var commentTemplate = Sitecore.Context.Database.GetTemplate("user defined/test templates/CustomComment");
            var categoryTemplate = Sitecore.Context.Database.GetTemplate("user defined/test templates/CustomCategory");

            var items = testRootItem.Axes.GetDescendants();
            foreach (var item in items)
            {
                using (new SecurityDisabler())
                {
                    if (item.TemplateID == Settings.BlogTemplateID)
                    {
                        item.ChangeTemplate(blogTemplate);
                        using (new EditContext(item))
                        {
                            BlogHomeItem blogItem = item;
                            blogItem.DefinedEntryTemplate.Field.Value = entryTemplate.InnerItem.ID.ToString();
                            blogItem.DefinedCommentTemplate.Field.Value = commentTemplate.InnerItem.ID.ToString();
                            blogItem.DefinedCategoryTemplate.Field.Value = categoryTemplate.InnerItem.ID.ToString();
                        }
                    }
                    else if (item.TemplateID == Settings.EntryTemplateID)
                    {
                        item.ChangeTemplate(entryTemplate);
                    }
                    else if (item.TemplateID == Settings.CommentTemplateID)
                    {
                        item.ChangeTemplate(commentTemplate);
                    }
                    else if (item.TemplateID == Settings.CategoryTemplateID)
                    {
                        item.ChangeTemplate(categoryTemplate);
                    }
                }
            }
        }

        public static void RemoveCustomTemplates(this Database database)
        {
            var templates = database.GetItem("/sitecore/templates/user defined/test templates");
            if (templates != null)
            {
                using (new SecurityDisabler())
                {
                    templates.Delete();
                }
            }
        }
    }
}