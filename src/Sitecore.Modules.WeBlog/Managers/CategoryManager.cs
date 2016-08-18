using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Managers
{
    /// <summary>
    /// Provides utilities for working with categories
    /// </summary>
    public class CategoryManager : ICategoryManager
    {
        /// <summary>
        /// The settings to use.
        /// </summary>
        protected IWeBlogSettings Settings = null;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="settings">The settings to use. If null, the default settings are used.</param>
        public CategoryManager(IWeBlogSettings settings = null)
        {
            Settings = settings ?? new WeBlogSettings();
        }

        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <returns>The list of categories</returns>
        public virtual CategoryItem[] GetCategories()
        {
            return GetCategories(Context.Item);
        }

        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <param name="item">The current item to search for a blog from</param>
        /// <returns>The list of categories</returns>
        public virtual CategoryItem[] GetCategories(Item item)
        {
            var categoryRoot = GetCategoryRoot(item);

            if (categoryRoot != null && categoryRoot.HasChildren)
            {
                var children = categoryRoot.GetChildren();
                return (from childItem in children
                        where childItem.TemplateIsOrBasedOn(Settings.CategoryTemplateIds)
                        && childItem.Versions.Count > 0
                        select new CategoryItem(childItem)).ToArray();
            }

            return new CategoryItem[0];
        }

        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <param name="Id">The ID of the current item to search for a blog from</param>
        /// <returns>The list of categories</returns>
        public virtual CategoryItem[] GetCategories(string Id)
        {
            var item = Context.Database.GetItem(Id);
            if (item != null)
                return GetCategories(item);

            return new CategoryItem[0];
        }

        /// <summary>
        /// Gets a category for a blog by name
        /// </summary>
        /// <param name="item">The blog or an item underneath the blog to search for the category within</param>
        /// <param name="name">The name of the category to locate</param>
        /// <returns>The category if found, otherwise null</returns>
        public virtual CategoryItem GetCategory(Item item, string name)
        {
            var categoryRoot = GetCategoryRoot(item);
            if (categoryRoot != null)
                return categoryRoot.Axes.GetChild(name);

            return null;
        }

        /// <summary>
        /// Gets the category root item for the current blog
        /// </summary>
        /// <param name="item">The item to search for the blog from</param>
        /// <returns>The category folder if found, otherwise null</returns>
        public virtual Item GetCategoryRoot(Item item)
        {
            var blogItem = item.FindAncestorByAnyTemplate(Settings.BlogTemplateIds);

            if (blogItem != null)
                return blogItem.Axes.GetChild("Categories");

            return null;
        }

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public virtual CategoryItem Add(string categoryName, Item item)
        {
            // Get all categories from current blog                
            var categories = GetCategories(item);

            // If there are categories, check if it already contains the categoryName
            if (categories.Any())
            {
                var existingCategory = categories.Where(x => x.Title.Raw.CompareTo(categoryName) == 0);
                if (existingCategory.Any())
                    return existingCategory.First();
            }

            // Category doesn't exist so create it
            var categoryRoot = GetCategoryRoot(item);
            if (categoryRoot == null)
            {
                categoryRoot = CreateCategoryRoot(item);
            }

            CategoryItem newCategory = ItemManager.AddFromTemplate(categoryName, Settings.CategoryTemplateIds.First(), categoryRoot);
            newCategory.BeginEdit();
            newCategory.Title.Field.Value = categoryName;
            newCategory.EndEdit();

            return newCategory;
        }

        protected virtual Item CreateCategoryRoot(Item item)
        {
            var blogHomeItem = ManagerFactory.BlogManagerInstance.GetCurrentBlog(item);
            return blogHomeItem.InnerItem.Add("Categories", new TemplateID(TemplateIDs.TemplateFolder));
        }

        /// <summary>
        /// Gets the categories for the blog entry given by ID.
        /// </summary>
        /// <param name="entryId">The ID of the blog entry to get the categories from.</param>
        /// <returns>The categories of the blog entry.</returns>
        public virtual CategoryItem[] GetCategoriesByEntryID(ID entryId)
        {
            var item = GetDatabase().GetItem(entryId);
            return GetCategoriesForEntry(item);
        }

        /// <summary>
        /// Gets the categories for the blog entry
        /// </summary>
        /// <param name="entry">The blog entry to get the categories for.</param>
        /// <returns>The categories of the blog entry.</returns>
        public virtual CategoryItem[] GetCategoriesForEntry(Item entry)
        {
            var categoryList = new List<CategoryItem>();

            if (entry == null)
                return categoryList.ToArray();

            var currentEntry = new EntryItem(entry);

            categoryList.AddRange(currentEntry.Category.ListItems.Select(cat => new CategoryItem(cat)));

            return categoryList.ToArray();
        }

        /// <summary>
        /// Gets the appropriate database to be reading data from
        /// </summary>
        /// <returns>The appropriate content database</returns>
        protected virtual Database GetDatabase()
        {
            return Context.ContentDatabase ?? Context.Database;
        }
    }
}