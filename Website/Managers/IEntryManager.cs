using System;
using System.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;

namespace Sitecore.Modules.WeBlog.Managers
{
    public interface IEntryManager
    {
        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="postID">The ID of the post to delete</param>
        /// <param name="db">The database to delete the entry from</param>
        /// <returns>True if the post was deleted, otherwise False</returns>
        bool DeleteEntry(string postID, Database db);

        /// <summary>
        /// Gets blog entries for the given blog
        /// </summary>
        /// <param name="blogItem">The blog item to retrieve the entries for</param>
        /// <returns>The entries for the current blog</returns>
        EntryItem[] GetBlogEntries(Item blogItem);

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="database">The database to get the blog from</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the given blog</returns>
        EntryItem[] GetBlogEntries(ID blogID, Database database, int maxNumber);

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="database">The database to get the blog from</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the given blog</returns>
        EntryItem[] GetBlogEntries(ID blogID, Database database, int maxNumber, string tag);

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blog">The blog item to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <param name="category">A category the entry must contain</param>
        /// <param name="minimumDate">The minimum date for entries</param>
        /// <param name="maximumDate">The maximum date for the entries</param>
        /// <returns></returns>
        EntryItem[] GetBlogEntries(Item blog, int maxNumber, string tag, string category, DateTime? minimumDate = null, DateTime? maximumDate = null);
        
        /// <summary>
        /// Gets the blog entries for a particular month and year.
        /// </summary>
        /// <param name="month">The month to get the entries for</param>
        /// <param name="year">The year to get the entries for</param>
        /// <returns>The entries for the month and year from the current blog</returns>
        EntryItem[] GetBlogEntriesByMonthAndYear(Item blog, int month, int year);

        /// <summary>
        /// Gets the most popular entries for the blog by the number of page views
        /// </summary>
        /// <param name="blogItem">The blog to find the most popular pages for</param>
        /// <param name="maxCount">The maximum number of entries to return</param>
        /// <returns>An array of EntryItem classes</returns>
        EntryItem[] GetPopularEntriesByView(Item blogItem, int maxCount);

        /// <summary>
        /// Gets the most popular entries for the blog by the number of comments on the entry
        /// </summary>
        /// <param name="blogItem">The blog to find the most popular pages for</param>
        /// <param name="maxCount">The maximum number of entries to return</param>
        /// <returns>An array of EntryItem classes</returns>
        EntryItem[] GetPopularEntriesByComment(Item blogItem, int maxCount);

        /// <summary>
        /// Gets the entry item for the current comment.
        /// </summary>
        /// <param name="commentItem">The comment item.</param>
        /// <returns></returns>
        EntryItem GetBlogEntryByComment(CommentItem commentItem);

        #region Deprecated

        /// <summary>
        /// Gets blog entries for the current blog
        /// </summary>
        /// <returns>The entries for the current blog</returns>
        /*[Obsolete("Use GetBlogEntries(Item) instead")] // deprecated 3.0
        EntryItem[] GetBlogEntries();*/

        /// <summary>
        /// Gets blog entries for the current blog up to the maximum number given
        /// </summary>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the current blog</returns>
        /*[Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        EntryItem[] GetBlogEntries(int maxNumber);*/

        /// <summary>
        /// Gets blog entries for the current blog containing the given tag
        /// </summary>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the current blog containing the given tag</returns>
        /*[Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        EntryItem[] GetBlogEntries(string tag);*/

        /// <summary>
        /// Gets blog entries for the current blog containing the given tag up to the maximum number given
        /// </summary>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the current blog containing the given tag</returns>
        /*[Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        EntryItem[] GetBlogEntries(int maxNumber, string tag);*/

        /// <summary>
        /// Gets the current context item as a blog entry
        /// </summary>
        /// <returns>The current blog entry</returns>
        /*[Obsolete("Use GetCurrentBlogEntry(Item) instead.")] // deprecated 3.0
        EntryItem GetCurrentBlogEntry();*/

        /// <summary>
        /// Gets the current context item as a blog entry
        /// </summary>
        /// <param name="item">The item to find the current entry item for</param>
        /// <returns>The current blog entry</returns>
        /*[Obsolete("Use EntryItem class instead.")] // deprecated 3.0
        EntryItem GetCurrentBlogEntry(Item item);*/

        /// <summary>
        /// Deletes a blog post
        /// </summary>
        /// <param name="postID">The ID of the post to delete</param>
        /// <returns>True if the post was deleted, otherwise False</returns>
        /*[Obsolete("Use DeleteEntry(string, Database) instead.")] // deprecated 3.0
        bool DeleteEntry(string postID);*/

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <returns>The entries for the given blog</returns>
        /*[Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        EntryItem[] GetBlogEntries(ID blogID, int maxNumber);*/

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blogID">The ID of the blog to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <returns>The entries for the given blog</returns>
        /*[Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        EntryItem[] GetBlogEntries(ID blogID, int maxNumber, string tag);*/

        /// <summary>
        /// Gets blog entries for the given blog up to the maximum number given
        /// </summary>
        /// <param name="blog">The blog item to get the entries for</param>
        /// <param name="maxNumber">The maximum number of entries to retrieve</param>
        /// <param name="tag">A tag the entry must contain</param>
        /// <param name="category">A category the entry must contain</param>
        /// <returns></returns>
        /*[Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead")] // deprecated in 2.4
        EntryItem[] GetBlogEntries(Item blog, int maxNumber, string tag, string category, string datePrefix = null);*/

        /// <summary>
        /// Gets the blog entries for a particular month and year.
        /// </summary>
        /// <param name="month">The month to get the entries for</param>
        /// <param name="year">The year to get the entries for</param>
        /// <returns>The entries for the month and year from the current blog</returns>
        /*[Obsolete("Use GetBlogEntriesByMonthAndYear(Item, int, int) instead.")] // deprecated 3.0
        EntryItem[] GetBlogEntriesByMonthAndYear(int month, int year);*/

        /// <summary>
        /// Gets the blog entry by categorie.
        /// </summary>
        /// <param name="blogId">The blog ID.</param>
        /// <param name="categorieName">Name of the categorie.</param>
        /// <returns>The entries of the blog tagged with the category</returns>
        /*[Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        EntryItem[] GetBlogEntryByCategorie(ID blogId, string categorieName);*/

        /// <summary>
        /// Gets the blog entry by categorie.
        /// </summary>
        /// <param name="BlogID">The blog ID.</param>
        /// <param name="CategorieName">Name of the categorie.</param>
        /// <returns></returns>
        /*[Obsolete("Use GetBlogEntries(Item, int, string, string, DateTime?, DateTime?) instead.")] // deprecated 3.0
        EntryItem[] GetBlogEntryByCategorie(ID blogId, ID categoryId);*/

        /// <summary>
        /// Makes the sorted post item list.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        /*[Obsolete("No longer used.")] // deprecated 3.0
        EntryItem[] MakeSortedEntriesList(IList array);*/
        #endregion
    }
}
