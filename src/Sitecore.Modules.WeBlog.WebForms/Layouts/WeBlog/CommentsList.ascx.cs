using System;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    public partial class BlogCommentsList : BaseEntrySublayout
    {
        public ICommentsListCore CommentsListCore { get; set; }

        public BlogCommentsList(ICommentsListCore commentsListCore = null)
        {
            CommentsListCore = commentsListCore ?? new CommentsListCore(CurrentBlog, CurrentEntry);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComments();
            Sitecore.Events.Event.Subscribe(Constants.Events.UI.COMMENT_ADDED, CommentAdded);
        }

        public override void Dispose()
        {
            // without unsubscribing there will be a memory leak from the subscription in the Page_Load event.
            Sitecore.Events.Event.Unsubscribe(Constants.Events.UI.COMMENT_ADDED, CommentAdded);
        }

        protected virtual void LoadComments(CommentItem addedComment = null)
        {
            // Comments enabled and exist?
            var showCommentsList = CurrentBlog.EnableComments.Checked && !CurrentEntry.DisableComments.Checked;
            if (!showCommentsList || ManagerFactory.CommentManagerInstance.GetCommentsCount(CurrentEntry) == 0)
            {
                if (CommentList != null)
                {
                    CommentList.Visible = false;
                }
            }
            else
            {
                if (ListViewComments != null)
                {
                    ListViewComments.DataSource = CommentsListCore.LoadComments(addedComment);
                    ListViewComments.DataBind();
                }
            }
        }

        protected virtual void CommentAdded(object sender, EventArgs e)
        {
            object[] parameters = (e as Sitecore.Events.SitecoreEventArgs).Parameters;
            if (parameters.Length > 0)
            {
                CommentItem added = parameters[0] as CommentItem;
                LoadComments(added);
            }
            else
            {
                LoadComments();
            }
        }
    }
}
