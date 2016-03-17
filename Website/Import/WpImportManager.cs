using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using System.Xml;
using System;
using Sitecore.Resources.Media;

namespace Sitecore.Modules.WeBlog.Import
{
    public static class WpImportManager
    {
        public static List<WpPost> Import(ID fileLocation, bool includeComments, bool includeCategories, bool includeTags)
        {
            Database master = Configuration.Factory.GetDatabase("master");
            Item sampleItem = master.GetItem(fileLocation);
            MediaItem sampleMedia = new MediaItem(sampleItem);

            XmlDocument xmdDoc = new XmlDocument();
            xmdDoc.Load(MediaManager.GetMedia(sampleMedia).GetStream().Stream);

            using (var nodeReader = new XmlNodeReader(xmdDoc))
            {
                nodeReader.MoveToContent();
                var xDocument = XDocument.Load(nodeReader);

                var posts = (from item in xDocument.Descendants("item")
                             select new WpPost(item, includeComments, includeCategories, includeTags)).ToList();
                return posts;
            }
       
        }

        public static BlogHomeItem CreateBlogRoot(Item root, string name, string email)
        {
            //TDOO: Move Content Database to db helpers
            var contentDatabase = ContentHelper.GetContentDatabase();

            BranchItem newBlog = contentDatabase.Branches.GetMaster(Settings.BlogBranchID);
            BlogHomeItem blogItem = root.Add(ItemUtil.ProposeValidItemName(name), newBlog);

            blogItem.BeginEdit();
            blogItem.Email.Field.Value = email;
            blogItem.EndEdit();

            return blogItem;
        }

        /// <summary>
        /// Imports the specified file.
        /// </summary>
        /// <param name="fileLocation">The file location.</param>
        /// <param name="includeComments">Determines if comments should be imported</param>
        /// <param name="includeCategories">Determines if categories should be imported</param>
        /// <param name="includeTags">Determines if tags should be imported</param>
        public static List<WpPost> Import(string fileLocation, bool includeComments, bool includeCategories, bool includeTags)
        {
            var nsm = new XmlNamespaceManager(new NameTable());
            nsm.AddNamespace("atom", "http://www.w3.org/2005/Atom");

            var parseContext = new XmlParserContext(null, nsm, null, XmlSpace.Default);
            using (var reader = XmlReader.Create(fileLocation, null, parseContext))
            {
                var doc = XDocument.Load(reader);

                var posts = (from item in doc.Descendants("item")
                             select new WpPost(item, includeComments, includeCategories, includeTags)).ToList();

                return posts;
            }
        }

        internal static void ImportPosts(Item blogItem, List<WpPost> listWordpressPosts, Database db, Action<string, int> logger = null)
        {
            BlogHomeItem customBlogItem = blogItem;
            var entryTemplate = TemplateManager.GetTemplate(customBlogItem.BlogSettings.EntryTemplateID, db);

            var processCount = 0;

            foreach (WpPost post in listWordpressPosts)
            {
                processCount++;

                if (!string.IsNullOrEmpty(post.Content))
                {
                    var name = ItemUtil.ProposeValidItemName(post.Title);

                    if (logger != null)
                        logger(name, processCount);

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

                    var publicationDate = DateUtil.ToIsoDate(post.PublicationDate);
                    entry.InnerItem.Fields[FieldIDs.Created].Value = publicationDate;
                    entry.InnerItem.Fields["Entry Date"].Value = publicationDate;
                    entry.EndEdit();
                }
            }
        }
    }
}