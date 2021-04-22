using Sitecore.Eventing;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using System;

namespace Sitecore.Modules.WeBlog.Pipelines.Initialize
{
    /// <summary>
    /// A pipeline processor which registers the event queue handler for <see cref="CommentSubmitted"/> events.
    /// </summary>
    public class RegisterCommentSubmittedHandler
    {
        /// <summary>
        /// Gets the <see cref="ICommentManager"/> used to add comments to entries.
        /// </summary>
        protected ICommentManager CommentManager { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="commentManager">The <see cref="ICommentManager"/> used to add comments to entries.</param>
        public RegisterCommentSubmittedHandler(ICommentManager commentManager)
        {
            CommentManager = commentManager;
        }

        public void Process(EventArgs args)
        {
            EventManager.Subscribe<CommentSubmitted>(comment =>
            {
                CommentManager.AddCommentToEntry(comment.EntryId, comment.Comment, comment.Language);
            });
        }
    }
}
