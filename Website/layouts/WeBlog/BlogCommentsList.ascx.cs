using System;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogCommentsList : BaseEntrySublayout
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Utilities.Presentation.SetProperties(this);
            LoadEntry();
        }

        protected virtual void LoadEntry()
        {
            // Comments enabled?
            if (CurrentEntry.DisableComments.Checked)
            {
                if(ListViewComments != null)
                    ListViewComments.Visible = false;
            }

            // Check for the existence of comments
            if (CommentManager.GetCommentsCount() != 0)
            {
                if (ListViewComments != null)
                {
                    ListViewComments.DataSource = CommentManager.GetEntryComments();
                    ListViewComments.DataBind();

                    var titleCommentControl = ListViewComments.FindControl("titleComments");
                    if(titleCommentControl != null)
                    {
                        ((FieldControl)titleCommentControl).Item = CurrentBlog.InnerItem;
                    }
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
                return baseUrl + "/" + Utilities.Crypto.GetMD5Hash(email) + ".jpg" +
                    "?s=" + CurrentBlog.GravatarSizeNumeric.ToString() +
                    "&d=" + CurrentBlog.DefaultGravatarStyle.Raw +
                    "&r=" + CurrentBlog.GravatarRating.Raw;
            }
            else
                return string.Empty;
        }
    }
}