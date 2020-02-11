namespace Sitecore.Modules.WeBlog.Model
{
    public class CommentContent : CommentReference
    {
        /// <summary>
        /// Gets or sets the name of the author of the comment.
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// Gets or sets the website of the author.
        /// </summary>
        public string AuthorWebsite { get; set; }

        /// <summary>
        /// Gets or sets the email address of the author.
        /// </summary>
        public string AuthorEmail { get; set; }

        /// <summary>
        /// Gets or sets the text of the comment.
        /// </summary>
        public string Text { get; set; }
    }
}