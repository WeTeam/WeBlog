using Sitecore.Data;
using System;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Configuration
{
    /// <summary>
    /// Defines the settings for the module.
    /// </summary>
    public interface IWeBlogSettings
    {
        /// <summary>
        /// Gets the name of the search index.
        /// </summary>
        string SearchIndexName { get; }

        /// <summary>
        /// Gets the size of the entries cache.
        /// </summary>
        long EntriesCacheSize { get; }

        /// <summary>
        /// Gets the IDs of the entry templates.
        /// </summary>
        IEnumerable<ID> EntryTemplateIds { get; }

        /// <summary>
        /// Gets the IDs of the comment templates.
        /// </summary>
        IEnumerable<ID> CommentTemplateIds { get; }

        /// <summary>
        /// Gets the IDs of the blog templates.
        /// </summary>
        IEnumerable<ID> BlogTemplateIds { get; }

        /// <summary>
        /// Gets the IDs of the category templates.
        /// </summary>
        IEnumerable<ID> CategoryTemplateIds { get; }

        /// <summary>
        /// Gets the IDs of the RSS Feed templates.
        /// </summary>
        IEnumerable<ID> RssFeedTemplateIds { get; }

        /// <summary>
        /// Gets the IDs of the blog branches
        /// </summary>
        IEnumerable<ID> BlogBranchIds { get; }

        /// <summary>
        /// Gets the URL for the Gravatar image service.
        /// </summary>
        string GravatarImageServiceUrl { get; }

        /// <summary>
        /// Gets the AddThis account name.
        /// </summary>
        string AddThisAccountName { get; }

        /// <summary>
        /// Gets the ShareThis publisher ID.
        /// </summary>
        string ShareThisPublisherId { get; }

        /// <summary>
        /// Gets the Akismet API key.
        /// </summary>
        [Obsolete("Use IWeBlogCommentSettings.AkismetAPIKey instead.")]
        string AkismetAPIKey { get; }

        /// <summary>
        /// Gets the content root path.
        /// </summary>
        string ContentRootPath { get; }

        /// <summary>
        /// Gets the size for the globalization cache.
        /// </summary>
        string GlobalizationCacheSize { get; }

        /// <summary>
        /// Gets the default dictionary path.
        /// </summary>
        string DictionaryPath { get; }

        /// <summary>
        /// Gets the dictionary entry template id.
        /// </summary>
        ID DictionaryEntryTemplateId { get; }

        /// <summary>
        /// Gets the ID of the workflow command to execute after creating a comment.
        /// </summary>
        [Obsolete("Use IWeBlogCommentSettings.CommentWorkflowCommandCreated instead.")]
        string CommentWorkflowCommandCreated { get; }

        /// <summary>
        /// Gets the ID of the workflow command to execute after a comment is classified as spam.
        /// </summary>
        [Obsolete("Use IWeBlogCommentSettings.CommentWorkflowCommandSpam instead.")]
        string CommentWorkflowCommandSpam { get; }

        /// <summary>
        /// Gets the cache size for the profanity filter.
        /// </summary>
        string ProfanityFilterCacheSize { get; }

        /// <summary>
        /// Gets the template ID for the profanity list.
        /// </summary>
        ID ProfanityListTemplateId { get; }

        /// <summary>
        /// Gets the date format setting.
        /// </summary>
        string DateFormat { get; }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the IBlogManager interface.
        /// </summary>
        string BlogManagerClass { get; }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ICategoryManager interface.
        /// </summary>
        string CategoryManagerClass { get; }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ICommentManager interface.
        /// </summary>
        string CommentManagerClass { get; }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the IEntryManager interface.
        /// </summary>
        string EntryManagerClass { get; }

        /// <summary>
        /// Gets the type that provides the concrete implementation of the ITagManager interface.
        /// </summary>
        string TagManagerClass { get; }

        /// <summary>
        /// Indicates whether to use the comment service or not.
        /// </summary>
        [Obsolete("Use the EventQueue for comment submission instead.")]
        bool CommentServiceEnabled { get;  }

        /// <summary>
        /// Maximum number of proposed tags returned in WeBlog Tags field
        /// </summary>
        int TagFieldMaxItemCount { get; }
    }
}
