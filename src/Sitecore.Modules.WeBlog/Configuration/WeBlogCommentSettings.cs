namespace Sitecore.Modules.WeBlog.Configuration
{
    public class WeBlogCommentSettings : IWeBlogCommentSettings
    {
        /// <summary>
        /// Gets the ID of the workflow command to execute after creating a comment.
        /// </summary>
        public string CommentWorkflowCommandCreated
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Comments.Workflow.Command.Created", ""); }
        }

        /// <summary>
        /// Gets the ID of the workflow command to execute after a comment is classified as spam.
        /// </summary>
        public string CommentWorkflowCommandSpam
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Comments.Workflow.Command.Spam"); }
        }

        /// <summary>
        /// Gets the Akismet API key.
        /// </summary>
        public string AkismetAPIKey
        {
            get { return Sitecore.Configuration.Settings.GetSetting("WeBlog.Akismet.APIKey"); }
        }

        /// <summary>
        /// Indicates whether submitted comments should be handled locally.
        /// </summary>
        public bool HandleSubmittedCommentsLocally
        {
            get { return Sitecore.Configuration.Settings.GetBoolSetting("WeBlog.CommentSubmitted.HandleLocally", false); }
        }
    }
}
