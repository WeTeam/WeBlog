using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Import
{
    public static class WpImportManager
    {

        /// <summary>
        /// Imports the specified file.
        /// </summary>
        /// <param name="fileLocation">The file location.</param>
        public static void Import(string fileLocation)
        {
            var xmlDoc = XDocument.Load(fileLocation);
            var items = xmlDoc.Descendants("item");

            var posts = (from item in xmlDoc.Descendants("item")
                                  select new WpPost(item)).ToList();
        }

        /// <summary>
        /// Imports the specified file.
        /// </summary>
        /// <param name="fileLocation">The file location.</param>
        public static List<WpPost> Import(string fileLocation, bool includeComments, bool includeCategories, bool includeTags)
        {
            var xmlDoc = XDocument.Load(fileLocation);
            var items = xmlDoc.Descendants("item");

            var posts = (from item in xmlDoc.Descendants("item")
                                  select new WpPost(item, includeComments, includeCategories, includeTags)).ToList();

            return posts;
        }

        internal static void ImportPosts(Data.Items.Item blogItem, List<WpPost> listWordpressPosts, Database db)
        {
            var entryTemplate = TemplateManager.GetTemplate(Settings.EntryTemplateId, db);

            foreach (WpPost post in listWordpressPosts)
            {
                if (!string.IsNullOrEmpty(post.Content))
                {
                    EntryItem entry = ItemManager.AddFromTemplate(ItemUtil.ProposeValidItemName(post.Title), entryTemplate.ID, blogItem);

                    entry.BeginEdit();
                    entry.Title.Field.Value = post.Title;
                    entry.Introduction.Field.Value = string.Empty;
                    entry.Content.Field.Value = post.Content;
                    entry.Tags.Field.Value = string.Join(", ", post.Tags.ToArray());

                    var categorieItems = new List<string>();

                    foreach (string categoryName in post.Categories)
                    {
                        var categoryItem = ManagerFactory.CategoryManagerInstance.Add(categoryName, blogItem);
                        categorieItems.Add(categoryItem.ID.ToString());
                    }
                 
                    if (categorieItems.Count > 0)
                    {
                        entry.Category.Field.Value = string.Join("|", categorieItems.ToArray());
                    }

                    foreach (WpComment wpComment in post.Comments)
                    {
                        ManagerFactory.CommentManagerInstance.AddCommentToEntry(entry.ID, wpComment);
                    }

                    entry.InnerItem.Fields[Sitecore.FieldIDs.Created].Value = Sitecore.DateUtil.ToIsoDate(post.PublicationDate);
                    entry.EndEdit();
                }
            }

        }
        
    }
}