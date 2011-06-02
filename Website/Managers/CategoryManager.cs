using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Items.Blog;
using Sitecore.Data.Managers;

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with categories
    /// </summary>
    public static class CategoryManager
    {
        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <returns>The list of categories</returns>
        public static CategoryItem[] GetCategories()
        {
            return GetCategories(Context.Item);
        }

        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <param name="item">The current item to search for a blog from</param>
        /// <returns>The list of categories</returns>
        public static CategoryItem[] GetCategories(Item item)
        {
            var categoryList = new List<CategoryItem>();
            var blogItem = item;
            var template = Context.Database.GetTemplate(Sitecore.Configuration.Settings.GetSetting("Blog.BlogTemplateID"));

            if(template != null)
            {
                //Check if current item equals blogroot
                while (blogItem != null && !Utilities.Items.TemplateIsOrBasedOn(blogItem, template))
                {
                    blogItem = blogItem.Parent;
                }

                if (blogItem != null)
                {
                    var categoriesFolder = blogItem.Axes.GetChild("Categories");
                    if (categoriesFolder != null && categoriesFolder.HasChildren)
                    {
                        var categoryTemplate = Context.Database.GetTemplate(Sitecore.Configuration.Settings.GetSetting("Blog.CategoryTemplateID"));
                        if (categoryTemplate != null)
                        {
                            foreach (Item category in categoriesFolder.GetChildren())
                            {
                                if (Utilities.Items.TemplateIsOrBasedOn(category, categoryTemplate) && category.Versions.Count > 0)
                                {
                                    categoryList.Add(new CategoryItem(category));
                                }
                            }
                        }
                    }
                }
            }
            return categoryList.ToArray();
        }

        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <param name="Id">The ID of the current item to search for a blog from</param>
        /// <returns>The list of categories</returns>
        public static CategoryItem[] GetCategories(string Id)
        {
            var item = Context.Database.GetItem(Id);
            if (item != null)
                return GetCategories(item);
            else
                return new CategoryItem[0];
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static CategoryItem Add(string categoryName, Item item)
        {
            Item blogItem = item;
            var template = Context.Database.GetTemplate(Sitecore.Configuration.Settings.GetSetting("Blog.BlogTemplateID"));
            

            if (template != null)
            {
                //Check if current item equals blogroot
                while (blogItem != null && !Utilities.Items.TemplateIsOrBasedOn(blogItem, template))
                {
                    blogItem = blogItem.Parent;
                }

                // Get all categories from current blog                
                CategoryItem[] categories = GetCategories(blogItem);
                
                // If there are categories, check if it already contains the categoryName
                if (categories != null)
                {
                    var resultList = categories.Where(x => x.Title.Raw.ToLower() == categoryName.ToLower());
                    var result = resultList == null ? null : resultList.First();
                    
                    // If category is found return ID
                    if (result != null)
                    {
                        return result;
                    }
                }

                // Category doesn't exist so create it
                var categoriesFolder = blogItem.Axes.GetChild("Categories");
                
                CategoryItem newCategory = ItemManager.AddFromTemplate(categoryName, new ID(CategoryItem.TemplateId), categoriesFolder);
                newCategory.BeginEdit();
                newCategory.Title.Field.Value = categoryName;
                newCategory.EndEdit();
                
                return newCategory;
            }
            return null;
        }

        /// <summary>
        /// Gets the categories for the blog entry given by ID
        /// </summary>
        /// <param name="EntryID">The ID of the blog entry to get teh categories from</param>
        /// <returns>The categories of the blog</returns>
        public static CategoryItem[] GetCategoriesByEntryID(ID EntryID)
        {
            var categoryList = new List<CategoryItem>();

            var item = Context.Database.GetItem(EntryID);

            if (item != null)
            {
                var currentEntry = new EntryItem(item);

                if (currentEntry != null)
                {
                    foreach (var cat in currentEntry.Category.ListItems)
                    {
                        categoryList.Add(new CategoryItem(cat));
                    }
                }
            }

            return categoryList.ToArray();
        }

        #region Obsolete Methods
        /// <summary>
        /// Gets the categories by entry ID.
        /// </summary>
        /// <param name="EntryID">The entry ID.</param>
        /// <returns></returns>
        [Obsolete("Use GetCategoriesByEntryID(ID EntryID).InnerItem instead")]
        public static Item[] GetCategoriesItemsByEntryID(ID EntryID)
        {
            return (from category in GetCategoriesByEntryID(EntryID) select category.InnerItem).ToArray();
        }
        #endregion
    }
}