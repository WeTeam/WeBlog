using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Collections;

namespace Sitecore.Modules.Eviblog.Managers
{
    public class CategoryManager
    {
        /// <summary>
        /// Gets the categories by blog ID.
        /// </summary>
        /// <returns></returns>
        public static List<Category> GetCategories()
        {
            //Get the current item
            Item Blog = Context.Item;

            //Check if current item equals blogroot
            if (Blog.TemplateID.ToString() != Settings.Default.BlogTemplateID)
            {
                while (Blog.TemplateID.ToString() != Settings.Default.BlogTemplateID)
                {
                    Blog = Blog.Parent;
                }
            }

            List<Category> CategoryList = new List<Category>();

            if(Blog.Axes.GetChild("Categories").HasChildren)
            {
                foreach (Item item in Blog.Axes.GetChild("Categories").Children)
                {
                    if (item.TemplateID.ToString() == Settings.Default.CategoryTemplateID &&
                        item.Versions.GetVersions().Length > 0)
                    {
                        CategoryList.Add(new Category(item));
                    }
                }
            }
            return CategoryList;
        }

        /// <summary>
        /// Gets the categories by blog ID.
        /// </summary>
        /// <returns></returns>
        public static List<Category> GetCategories(string ID)
        {
            //Get the current item
            Item Blog = BlogManager.GetBlogByID(new ID(ID));

            //Check if current item equals blogroot
            if (Blog.TemplateID.ToString() != Settings.Default.BlogTemplateID)
            {
                while (Blog.TemplateID.ToString() != Settings.Default.BlogTemplateID)
                {
                    Blog = Blog.Parent;
                }
            }

            List<Category> CategoryList = new List<Category>();

            foreach (Item item in Blog.Axes.GetChild("Categories").Children)
            {
                if (item.TemplateID.ToString() == Settings.Default.CategoryTemplateID &&
                    item.Versions.GetVersions().Length > 0)
                {
                    CategoryList.Add(new Category(item));
                }
            }

            return CategoryList;
        }

        /// <summary>
        /// Gets the categories by entry ID.
        /// </summary>
        /// <param name="EntryID">The entry ID.</param>
        /// <returns></returns>
        public static List<Category> GetCategoriesByEntryID(ID EntryID)
        {
            List<Category> CategoryList = new List<Category>();

            Entry currentEntry = new Entry(Context.Database.GetItem(EntryID));

            foreach (string cat in currentEntry.CategoriesID)
            {
                CategoryList.Add(new Category(Context.Database.GetItem(new ID(cat))));
            }

            return CategoryList;
        }

        /// <summary>
        /// Gets the categories by entry ID.
        /// </summary>
        /// <param name="EntryID">The entry ID.</param>
        /// <returns></returns>
        public static List<Item> GetCategoriesItemsByEntryID(ID EntryID)
        {
            List<Item> CategoryList = new List<Item>();

            Entry currentEntry = new Entry(Context.Database.GetItem(EntryID));

            foreach (string cat in currentEntry.CategoriesID)
            {
                CategoryList.Add(Context.Database.GetItem(new ID(cat)));
            }

            return CategoryList;
        }
    }
}