using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogCommentsList : BaseEntrySublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComments();
            Sitecore.Events.Event.Subscribe(Constants.Events.UI.COMMENT_ADDED, new EventHandler(this.CommentAdded));
        }
        
        public override void Dispose()
        {
            // without unsubscribing there will be a memory leak from the subscription in the Page_Load event.
            Sitecore.Events.Event.Unsubscribe(Constants.Events.UI.COMMENT_ADDED, new EventHandler(this.CommentAdded));
        } 
        

        protected virtual void LoadComments()
        {
            LoadComments(null);
        }

        protected virtual void LoadComments(CommentItem addedComment)
        {
            // Comments enabled and exist?
            if (CurrentEntry.DisableComments.Checked || ManagerFactory.CommentManagerInstance.GetCommentsCount() == 0)
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
                    CommentItem[] comments = ManagerFactory.CommentManagerInstance.GetEntryComments();
                    //if a comment has been added but is not coming back yet (i.e. being indexed), fake it
                    if (addedComment != null && comments.Count(comment => comment.ID == addedComment.ID) == 0)
                    {
                        List<CommentItem> newList = new List<CommentItem>();
                        newList.Add(addedComment);
                        newList.AddRange(comments);
                        comments = newList.ToArray();
                    }
                    ListViewComments.DataSource = comments;
                    ListViewComments.DataBind();
                }
            }
        }

        /// <summary>
        /// Get the URL for the user's gravatar image
        /// </summary>
        /// <param name="email">The email address of the user to get the gravatar for</param>
        /// <returns>The URL for the gravatar image</returns>
        protected virtual string GetGravatarUrl(string email)
        {
            var baseUrl = Settings.GravatarImageServiceUrl;
            if (!string.IsNullOrEmpty(baseUrl))
            {
                return baseUrl + "/" + Crypto.GetMD5Hash(email) + ".jpg" +
                    "?s=" + CurrentBlog.GravatarSizeNumeric.ToString() +
                    "&d=" + CurrentBlog.DefaultGravatarStyle.Raw +
                    "&r=" + CurrentBlog.GravatarRating.Raw;
            }
            else
                return string.Empty;
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
