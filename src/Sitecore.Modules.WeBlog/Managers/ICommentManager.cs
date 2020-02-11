﻿using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Model;

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
        /// Gets the number of comments under the given entry.
        /// </summary>
        /// <param name="entry">The entry to get the comment count for</param>
        /// <param name="language">The language to check comments in</param>
        /// <returns>The number of comments</returns>
        int GetCommentsCount(Item entry);
        
        /// <summary>
        /// Gets the comments for the blog entry
        /// </summary>
        /// <param name="blogItem">The blog to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <param name="language">The language to get the comments in</param>
        /// <returns>The comments for the blog entry</returns>
        IList<CommentReference> GetBlogComments(Item blogItem, int maximumCount);

        /// <summary>
        /// Gets the <see cref="ItemUri"/>s for the entries that have the most comments.
        /// </summary>
        /// <param name="blogItem">The root blog item to search below.</param>
        /// <param name="maximumCount">The maximum number of entry <see cref="ItemUri"/>s to return.</param>
        /// <returns>The list of <see cref="ItemUri"/>s for the entries.</returns>
        IList<ItemUri> GetMostCommentedEntries(Item blogItem, int maximumCount);

        /// <summary>
        /// Gets the comments for the given blog entry
        /// </summary>
        /// <param name="entryItem">The blog entry to get the comments for</param>
        /// <param name="maximumCount">The maximum number of comments to retrieve</param>
        /// <returns>The comments for the blog entry</returns>
        IList<CommentReference> GetEntryComments(Item entryItem, int maximumCount);

        /// <summary>
        /// Get the <see cref="CommentContent"/> for a <see cref="CommentReference"/>.
        /// </summary>
        /// <param name="commentReference">The <see cref="CommentReference"/> which identifies the comment.</param>
        /// <returns>The <see cref="CommentContent"/> for the <see cref="CommentReference"/>.</returns>
        CommentContent GetCommentContent(CommentReference commentReference);
    }
}
