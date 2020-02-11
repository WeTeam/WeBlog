using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Modules.WeBlog.Search;

namespace Sitecore.Modules.WeBlog.Managers
{
    public interface IEntryManager
    {
        /// <summary>
        /// Deletes a blog post.
        /// </summary>
        /// <param name="postId">The ID of the post to delete.</param>
        /// <param name="db">The database to delete the entry from.</param>
        /// <returns>True if the post was deleted, otherwise False.</returns>
        bool DeleteEntry(string postId, Database db);

        /// <summary>
        /// Gets blog entries for the given blog which meet the criteria.
        /// </summary>
        /// <param name="blogRootItem">The root item of the blog to retrieve the entries for.</param>
        /// <param name="criteria">The criteria the entries should meet.</param>
        /// <param name="resultOrder">The ordering of the results.</param>
        /// <returns>The entries matching the criteria.</returns>
        SearchResults<Entry> GetBlogEntries(Item blogRootItem, EntryCriteria criteria, ListOrder resultOrder);

        /// <summary>
        /// Gets the most popular entries for the blog by the number of page views.
        /// </summary>
        /// <param name="blogItem">The blog to find the most popular pages for.</param>
        /// <param name="maxCount">The maximum number of entries to return.</param>
        /// <returns>The <see cref="ItemUri"/> for the entry items.</returns>
        IList<ItemUri> GetPopularEntriesByView(Item blogItem, int maxCount);

        /// <summary>
        /// Gets the most popular entries for the blog by the number of comments on the entry.
        /// </summary>
        /// <param name="blogItem">The blog to find the most popular pages for.</param>
        /// <param name="maxCount">The maximum number of entries to return.</param>
        /// <returns>The <see cref="ItemUri"/> for the entry items.</returns>
        IList<ItemUri> GetPopularEntriesByComment(Item blogItem, int maxCount);

        /// <summary>
        /// Gets the entry item for the given comment.
        /// </summary>
        /// <param name="commentUri">The <see cref="ItemUri"/> of the comment item.</param>
        /// <returns>The <see cref="EntryItem"/> that owns the comment.</returns>
        EntryItem GetBlogEntryItemByCommentUri(ItemUri commentUri);
    }
}
