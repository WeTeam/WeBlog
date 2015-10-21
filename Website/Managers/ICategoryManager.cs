using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Managers
{
    public interface ICategoryManager
    {
        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <returns>The list of categories</returns>
        CategoryItem[] GetCategories();

        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <param name="item">The current item to search for a blog from</param>
        /// <returns>The list of categories</returns>
        CategoryItem[] GetCategories(Item item);

        /// <summary>
        /// Gets the categories for the current blog
        /// </summary>
        /// <param name="Id">The ID of the current item to search for a blog from</param>
        /// <returns>The list of categories</returns>
        CategoryItem[] GetCategories(string Id);

        /// <summary>
        /// Gets a category for a blog by name
        /// </summary>
        /// <param name="item">The blog or an item underneath the blog to search for the category within</param>
        /// <param name="name">The name of the category to locate</param>
        /// <returns>The category if found, otherwise null</returns>
        CategoryItem GetCategory(Item item, string name);

        /// <summary>
        /// Gets the category root item for the current blog
        /// </summary>
        /// <param name="item">The item to search for the blog from</param>
        /// <returns>The category folder if found, otherwise null</returns>
        Item GetCategoryRoot(Item item);

        /// <summary>
        /// Adds a new category.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        CategoryItem Add(string categoryName, Item item);

        /// <summary>
        /// Gets the categories for the blog entry given by ID
        /// </summary>
        /// <param name="entryId">The ID of the blog entry to get teh categories from</param>
        /// <returns>The categories of the blog</returns>
        CategoryItem[] GetCategoriesByEntryID(ID entryId);
    }
}
