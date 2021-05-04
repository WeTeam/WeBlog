namespace Sitecore.Modules.WeBlog.Configuration
{
    /// <summary>
    /// Defines the comment settings for WeBlog.
    /// </summary>
    public interface IWeBlogCommentSettings
    {
        /// <summary>
        /// Gets the ID of the workflow command to execute after creating a comment.
        /// </summary>
        string CommentWorkflowCommandCreated { get; }

        /// <summary>
        /// Gets the ID of the workflow command to execute after a comment is classified as spam.
        /// </summary>
        string CommentWorkflowCommandSpam { get; }

        /// <summary>
        /// Gets the Akismet API key.
        /// </summary>
        string AkismetAPIKey { get; }

        /// <summary>
        /// Indicates whether submitted comments should be handled locally.
        /// </summary>
        bool HandleSubmittedCommentsLocally { get; }
    }
}
