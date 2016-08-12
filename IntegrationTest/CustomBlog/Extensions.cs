using System.IO;
using System.Web;
using Sitecore.Caching;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.SecurityModel;

namespace Sitecore.Modules.WeBlog.Test.CustomBlog
{
    public static class Extensions
    {
        public static void SetupCustomBlogs(this Database database, Item testRoot)
        {
            var templateRoot = database.GetItem("/sitecore/templates/user defined");
            using (new SecurityDisabler())
            {
                using (new EventDisabler())
                {
                    templateRoot.Paste(File.ReadAllText(HttpContext.Current.Server.MapPath(@"~\test data\custom blog templates.xml")), false, PasteMode.Overwrite);
                    CacheManager.ClearAllCaches();
                }
            }

            var blogTemplate = Context.Database.GetTemplate("user defined/test templates/CustomBlog");
            var entryTemplate = Context.Database.GetTemplate("user defined/test templates/CustomEntry");
            var commentTemplate = Context.Database.GetTemplate("user defined/test templates/CustomComment");
            var categoryTemplate = Context.Database.GetTemplate("user defined/test templates/CustomCategory");

            var items = testRoot.Axes.GetDescendants();
            foreach (var item in items)
            {
              // Ensure the item exists in this language, or find which language it does exist in
              var wipItem = item;
              if (wipItem.Versions.Count == 0)
              {
                foreach (var language in wipItem.Languages)
                {
                  wipItem = wipItem.Database.GetItem(wipItem.ID, language);
                  if (wipItem.Versions.Count > 0)
                    break;
                }
              }

              if (wipItem.Versions.Count == 0)
                continue;

                using (new SecurityDisabler())
                {
                  
                  {
                    if (wipItem.TemplateID == Settings.BlogTemplateID)
                    {
                      wipItem.ChangeTemplate(blogTemplate);
                      using (new EditContext(wipItem))
                      {
                        BlogHomeItem blogItem = wipItem;
                        blogItem.DefinedEntryTemplate.Field.Value = entryTemplate.InnerItem.ID.ToString();
                        blogItem.DefinedCommentTemplate.Field.Value = commentTemplate.InnerItem.ID.ToString();
                        blogItem.DefinedCategoryTemplate.Field.Value = categoryTemplate.InnerItem.ID.ToString();
                      }
                    }
                    else if (wipItem.TemplateID == Settings.EntryTemplateID)
                    {
                      //using (new EventDisabler())
                      wipItem.ChangeTemplate(entryTemplate);
                    }
                    else if (wipItem.TemplateID == Settings.CommentTemplateID)
                    {
                      //using (new EventDisabler())
                      wipItem.ChangeTemplate(commentTemplate);
                    }
                    else if (wipItem.TemplateID == Settings.CategoryTemplateID)
                    {
                      //using (new EventDisabler())
                      wipItem.ChangeTemplate(categoryTemplate);
                    }
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