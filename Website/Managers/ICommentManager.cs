﻿using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Items.WeBlog;

namespace Sitecore.Modules.WeBlog.Managers
{
    public interface ICommentManager
    {
        /// <summary>
        /// Adds a comment to a blog
        /// </summary>
        /// <param name="entryId">The ID of the entry to add the comment to</param>
        /// <param name="comment">The comment to add to the entry</param>
        /// <param name="language">The language to create the comment in</param>
        /// <returns>The ID of the created comment item, or ID.Null if creation failed</returns>
        ID AddCommentToEntry(ID entryId, Model.Comment comment,  Language language = null);

        /// <summary>
        /// Submit a comment for inclusion on a post. This method will either update Sitecore or submit the comment through the comment service, depending on settings
        /// </summary>
        /// <param name="entryId">The ID of the entry to add the comment to</param>
        /// <param name="comment">The comment to add to the entry</param>
        /// <param name="language">The language to create the comment in</param>
        /// <returns>The ID of the created comment item, or ID.Null if creation failed</returns>
        ID SubmitComment(ID entryId, Model.Comment comment, Language language = null);

        /// <summary>
        /// Gets the number of comments for the current entry.
        /// </summary>
        /// <param name="language">The language to check comments in</param>
        /// <returns>The number of comments</returns>
        int GetCommentsCount(Language language = null);

        /// <summary>
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entry">The entry to get the comment count for</param>
        /// <param name="language">The language to check comments in</param>
        /// <returns>The number of comments</returns>
        int GetCommentsCount(Item entry);

        /// <summary>
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entryId">The ID of the entry to get the comment count for</param>
        /// <param name="language">The language to check comments in</param>
        /// <returns>The number of comments in the specified language</returns>
        int GetCommentsCount(ID entryId, Language language = null);

        /// <summary>
        /// Gets the comments for the blog entry
        /// </summary>
        /// <param name="blogId">The ID of the blog to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="language">The language to get the comments in</param>
        /// <returns>The comments for the blog entry</returns>
        CommentItem[] GetCommentsByBlog(ID blogId, int maximumCount, Language language = null);
        
        /// <summary>
        /// Gets the comments for the blog entry
        /// </summary>
        /// <param name="blogItem">The blog to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="language">The language to get the comments in</param>
        /// <returns>The comments for the blog entry</returns>
        CommentItem[] GetCommentsByBlog(Item blogItem, int maximumCount);

        /// <summary>
        /// Gets the comments for the current blog entry
        /// </summary>
        /// <param name="language">The language to get comments in</param>
        /// <returns>The comments for the blog entry</returns>
        CommentItem[] GetEntryComments(Language language = null);

        /// <summary>
        /// Gets the comments for the current blog entry
        /// </summary>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="language">The language to get the comments in</param>
        /// <returns>The comments for the blog entry</returns>
        CommentItem[] GetEntryComments(int maximumCount, Language language = null);

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <returns>The comments for the blog entry</returns>
        CommentItem[] GetEntryComments(Item entryItem);

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        CommentItem[] GetEntryComments(Item entryItem, int maximumCount);

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="item">The item to get the comments under</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="sort">Determines if the comments should be sorted</param>
        /// <param name="reverse">Determines if the sort order should be reversed</param>
        /// <returns>The comments which are decendants of the given item</returns>
        CommentItem[] GetCommentsFor(Item item, int maximumCount, bool sort = false, bool reverse = false);

        /// <summary>
        /// Sort the comments list using the CommentDateComparerDesc comparer
        /// </summary>
        /// <param name="array">The comments to sort</param>
        /// <returns>A sorted list of comments</returns>
        CommentItem[] MakeSortedCommentsList(System.Collections.IList array);
    }
}
