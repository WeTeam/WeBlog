using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Modules.WeBlog.Items.WeBlog;

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
            XDocument xmlDoc = XDocument.Load(fileLocation);
            var items = xmlDoc.Descendants("item");

            List<WpPost> posts = (from item in xmlDoc.Descendants("item")
                                  select new WpPost(item)).ToList();
        }

        /// <summary>
        /// Imports the specified file.
        /// </summary>
        /// <param name="fileLocation">The file location.</param>
        public static List<WpPost> Import(string fileLocation, bool includeComments, bool includeCategories, bool includeTags)
        {
            XDocument xmlDoc = XDocument.Load(fileLocation);
            var items = xmlDoc.Descendants("item");

            List<WpPost> posts = (from item in xmlDoc.Descendants("item")
                                  select new WpPost(item, includeComments, includeCategories, includeTags)).ToList();

            return posts;
        }

        internal static void ImportPosts(Data.Items.Item blogItem, List<WpPost> listWordpressPosts, Database db)
        {
            Template entryTemplate = TemplateManager.GetTemplate(Settings.EntryTemplateId, db);

            foreach (WpPost post in listWordpressPosts)
            {
                EntryItem entry = ItemManager.AddFromTemplate(Utilities.Items.MakeSafeItemName(post.Title), entryTemplate.ID, blogItem);

                entry.BeginEdit();
                entry.Title.Field.Value = post.Title;
                entry.Content.Field.Value = post.Content;
                entry.Tags.Field.Value = string.Join(", ", post.Tags.ToArray());

                List<string> categorieItems = new List<string>();

                foreach (string categoryName in post.Categories)
                {
                    CategoryItem categoryItem = Sitecore.Modules.WeBlog.Managers.CategoryManager.Add(categoryName, blogItem);
                    categorieItems.Add(categoryItem.ID.ToString());
                }
                if (categorieItems.Count > 0)
                {
                    entry.Category.Field.Value = string.Join("|", categorieItems.ToArray());
                }

                foreach (WpComment wpComment in post.Comments)
                {
                    Sitecore.Modules.WeBlog.Managers.CommentManager.AddComment(entry, wpComment);
                }

                entry.InnerItem.Fields[Sitecore.FieldIDs.Created].Value = Sitecore.DateUtil.ToIsoDate(post.PublicationDate);
                entry.EndEdit();

            }

        }
        
    }
}