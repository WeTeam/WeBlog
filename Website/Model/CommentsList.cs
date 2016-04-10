using System;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Model
{
    public class CommentsList : BlogRenderingModelBase
    {
        public CommentItem[] Comments { get; set; }
        public bool EnableGravatar { get; set; }
        public int GravatarSizeNumeric { get; set; }
        public bool ShowEmailWithinComments { get; set; }
        public bool ShowCommentsList { get; set; }
        public Func<string, string> GetGravatarUrl { get; set; }

        protected ICommentsListCore CommentsListCore { get; set; }

        public CommentsList() : this(null) { }

        public CommentsList(ICommentsListCore commentsListCore)
        {
            CommentsListCore = commentsListCore ?? new CommentsListCore(CurrentBlog);
            Comments = new CommentItem[0];
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            ShowCommentsList = CurrentBlog.EnableComments.Checked && !CurrentEntry.DisableComments.Checked;
            if (!ShowCommentsList || ManagerFactory.CommentManagerInstance.GetCommentsCount() == 0)
            {
                return;
            }
            Comments = CommentsListCore.LoadComments();
            GetGravatarUrl = CommentsListCore.GetGravatarUrl;
            EnableGravatar = CurrentBlog.EnableGravatar.Checked;
            GravatarSizeNumeric = CurrentBlog.GravatarSizeNumeric;
            ShowEmailWithinComments = CurrentBlog.ShowEmailWithinComments.Checked;
        }
    }
}