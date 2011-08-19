using System;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Items.Blog;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Globalization;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogEntry : BaseSublayout
    {
        MD5CryptoServiceProvider m_hasher = null;

        /// <summary>
        /// Gets or sets the current entry to display from
        /// </summary>
        public EntryItem CurrentEntry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CSS class to set on the message panel for success messages
        /// </summary>
        public string SuccessCssClass
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CSS class to set on the message panel for error messages
        /// </summary>
        public string ErrorCssClass
        {
            get;
            set;
        }

        public BlogEntry()
        {
            m_hasher = new MD5CryptoServiceProvider();
        }

        #region Page methods

        protected void Page_Load(object sender, EventArgs e)
        {
            SuccessCssClass = "successtext";
            ErrorCssClass = "errortext";

            Utilities.Presentation.SetProperties(this);

            CurrentEntry = new EntryItem(Sitecore.Context.Item);

            LoadEntry();
            Page.Title = CurrentEntry.Title.Text + " | " + CurrentBlog.Title.Text;

            var maxEntryImage = CurrentBlog.MaximumEntryImageSizeDimension;
            if(maxEntryImage != Size.Empty)
            {
                EntryImage.MaxWidth = maxEntryImage.Width;
                EntryImage.MaxHeight = maxEntryImage.Height;
            }
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        protected virtual void LoadEntry()
        {
            // Create entry of current item
            EntryItem current = new EntryItem(Sitecore.Context.Item);

            // Fill categories
            ListViewCategories.DataSource = CategoryManager.GetCategoriesByEntryID(current.ID);
            ListViewCategories.DataBind();

            //TODO Create edit possibilities for assigning categories on frontend

            #region Comments

            // Comments enabled?
            if (CurrentEntry.DisableComments.Checked)
            {
                ListViewComments.Visible = false;
                CommentsPanel.Visible = false;
            }
            else
            {
                ValidationSummaryComments.HeaderText = Translate.Text("REQUIRED_FIELDS");
                buttonSaveComment.Text = Translate.Text("POST");
            }

            // Check for the existence of comments
            if (CommentManager.GetCommentsCount() != 0)
            {
                ListViewComments.DataSource = CommentManager.GetEntryComments();
                ListViewComments.DataBind();
            }

            #endregion

            LoadTags(current);
        }

        #endregion

        /// <summary>
        /// Load the tags for the current entry
        /// </summary>
        /// <param name="entry">The entry to laod the tags from</param>
        protected virtual void LoadTags(EntryItem entry)
        {
            if (!Sitecore.Context.PageMode.IsPageEditorEditing)
            {
                var tags = TagManager.GetTagsByEntry(entry);
                var list = LoginViewTags.FindControl("TagList") as Repeater;

                if (list != null)
                {
                    list.DataSource = from tag in tags select tag.Key;
                    list.DataBind();
                }
            }
        }

        /// <summary>
        /// Get the URL for a tag
        /// </summary>
        /// <param name="tag">The tag to get the URL for</param>
        /// <returns>The URL to the tag</returns>
        protected virtual string GetTagUrl(string tag)
        {
            return LinkManager.GetItemUrl(Sitecore.Context.Item.Parent) + "?tag=" + tag; ;
        }

        #region Eventhandlers
        protected void buttonSaveComment_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Model.Comment comment = new Model.Comment()
                {
                    AuthorName = txtCommentName.Text,
                    AuthorEmail = txtCommentEmail.Text,
                    AuthorWebsite = txtCommentWebsite.Text,
                    AuthorIP = Context.Request.UserHostAddress,
                    Text = txtCommentText.Text
                };

                var submissionResult = CommentManager.SubmitComment(Sitecore.Context.Item.ID, comment); ;
                if (submissionResult.IsNull)
                    SetErrorMessage("An error occurred during comment submission. Please try again later.");
                else
                {
                    SetSuccessMessage("Thank you for your comment.");
                    ResetCommentFields();
                }
            }
        }

        /// <summary>
        /// Display an error message on the UI
        /// </summary>
        /// <param name="message">The message to display</param>
        protected virtual void SetErrorMessage(string message)
        {
            MessagePanel.CssClass = ErrorCssClass;
            Message.Text = message;
        }

        /// <summary>
        /// Display a success message on the UI
        /// </summary>
        /// <param name="message"></param>
        protected virtual void SetSuccessMessage(string message)
        {
            MessagePanel.CssClass = SuccessCssClass;
            Message.Text = message;
        }

        private void ResetCommentFields()
        {
            txtCommentName.Text = string.Empty;
            txtCommentEmail.Text = string.Empty;
            txtCommentWebsite.Text = string.Empty;
            txtCommentText.Text = string.Empty;
        }

        #endregion

        /// <summary>
        /// Get the URL for an item
        /// </summary>
        /// <param name="item">The item to get the URL for</param>
        /// <returns>The URL for the item if valid, otherwise an empty string</returns>
        protected virtual string GetItemUrl(Item item)
        {
            if (item != null)
                return LinkManager.GetItemUrl(item);
            else
                return string.Empty;
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