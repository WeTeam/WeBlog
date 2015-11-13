﻿using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with categories
    /// </summary>
    public class CategoryManager : ICategoryManager
    {
        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <returns>The list of categories</returns>
        public CategoryItem[] GetCategories()
        {
            return GetCategories(Context.Item);
        }

        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <param name="item">The current item to search for a blog from</param>
        /// <returns>The list of categories</returns>
        public CategoryItem[] GetCategories(Item item)
        {
            var categoryList = new List<CategoryItem>();
            var categoryRoot = GetCategoryRoot(item);
            
            if (categoryRoot != null && categoryRoot.HasChildren)
            {
                var categoryTemplate = GetDatabase().GetTemplate(Settings.CategoryTemplateID);
                if (categoryTemplate != null)
                {
                    foreach (Item category in categoryRoot.GetChildren())
                    {
                        if (category.TemplateIsOrBasedOn(categoryTemplate) && category.Versions.Count > 0)
                        {
                            categoryList.Add(new CategoryItem(category));
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
        public CategoryItem[] GetCategories(string Id)
        {
            var item = Context.Database.GetItem(Id);
            if (item != null)
                return GetCategories(item);
            else
                return new CategoryItem[0];
        }

        /// <summary>
        /// Gets a category for a blog by name
        /// </summary>
        /// <param name="item">The blog or an item underneath the blog to search for the category within</param>
        /// <param name="name">The name of the category to locate</param>
        /// <returns>The category if found, otherwise null</returns>
        public CategoryItem GetCategory(Item item, string name)
        {
            var categoryRoot = GetCategoryRoot(item);
            return categoryRoot.Axes.GetChild(name);
        }

        /// <summary>
        /// Gets the category root item for the current blog
        /// </summary>
        /// <param name="item">The item to search for the blog from</param>
        /// <returns>The category folder if found, otherwise null</returns>
        public Item GetCategoryRoot(Item item)
        {
            var blogItem = item;
            var template = GetDatabase().GetTemplate(Settings.BlogTemplateID);

            if (template != null)
            {
                //Check if current item equals blogroot
                while (blogItem != null && !blogItem.TemplateIsOrBasedOn(template))
                {
                    blogItem = blogItem.Parent;
                }

                if (blogItem != null)
                {
                    return blogItem.Axes.GetChild("Categories");
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public CategoryItem Add(string categoryName, Item item)
        {
            Item blogItem = item;
            var template = GetDatabase().GetTemplate(Settings.BlogTemplateID);

            if (template != null)
            {
                //Check if current item equals blogroot
                while (blogItem != null && !blogItem.TemplateIsOrBasedOn(template))
                {
                    blogItem = blogItem.Parent;
                }

                // Get all categories from current blog                
                CategoryItem[] categories = GetCategories(blogItem);
                
                // If there are categories, check if it already contains the categoryName
                if (categories.Count() > 0)
                {
                    var resultList = categories.Where(x => x.Title.Raw.ToLower() == categoryName.ToLower());
                    var result = resultList.Count() == 0 ? null : resultList.First();
                    
                    // If category is found return ID
                    if (result != null)
                    {
                        return result;
                    }
                }

                // Category doesn't exist so create it
                if (blogItem == null) return null;
                
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
        /// <param name="entryId">The ID of the blog entry to get teh categories from</param>
        /// <returns>The categories of the blog</returns>
        public CategoryItem[] GetCategoriesByEntryID(ID entryId)
        {
            var categoryList = new List<CategoryItem>();

            var item = GetDatabase().GetItem(entryId);

            if (item == null) return categoryList.ToArray();

            var currentEntry = new EntryItem(item);

            categoryList.AddRange(currentEntry.Category.ListItems.Select(cat => new CategoryItem(cat)));

            return categoryList.ToArray();
        }

        /// <summary>
        /// Gets the appropriate database to be reading data from
        /// </summary>
        /// <returns>The appropriate content database</returns>
        protected Database GetDatabase()
        {
            return Context.ContentDatabase ?? Context.Database;
        }
    }
}