using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Mvc.Presentation;

namespace Sitecore.Modules.WeBlog.Mvc.Model
{
    public class CommentsList : BlogRenderingModelBase
    {
        public IList<CommentContent> Comments { get; set; }
        public bool EnableGravatar { get; set; }
        public int GravatarSizeNumeric { get; set; }
        public bool ShowEmailWithinComments { get; set; }
        public bool ShowCommentsList { get; set; }
        public Func<string, string> GetGravatarUrl { get; set; }

        protected ICommentsListCore CommentsListCore { get; set; }

        protected ICommentManager CommentManager { get; set; }

        public CommentsList() : this(null, null) { }

        public CommentsList(ICommentsListCore commentsListCore, ICommentManager commentManager)
        {
            CommentsListCore = commentsListCore ?? new CommentsListCore(CurrentBlog, CurrentEntry);
            CommentManager = commentManager ?? ManagerFactory.CommentManagerInstance;
            Comments = new CommentContent[0];
        }

        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            ShowCommentsList = CurrentBlog.EnableComments.Checked && !CurrentEntry.DisableComments.Checked;
            var commentReferences = CommentsListCore.LoadComments();

            Comments = commentReferences.Select(x => CommentManager.GetCommentContent(x)).ToList();

            if (!ShowCommentsList || Comments.Count == 0)
            {
                return;
            }
            
            GetGravatarUrl = CommentsListCore.GetGravatarUrl;
            EnableGravatar = CurrentBlog.EnableGravatar.Checked;
            GravatarSizeNumeric = CurrentBlog.GravatarSizeNumeric;
            ShowEmailWithinComments = CurrentBlog.ShowEmailWithinComments.Checked;
        }

        public string GetAuthorWebsite(CommentReference comment)
        {
            var content = CommentManager.GetCommentContent(comment);
            return content?.AuthorWebsite;
        }
    }
}