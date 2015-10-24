using System;
using System.Linq;
using System.Web.UI;
using Sitecore.Modules.WeBlog.Globalization;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Layouts
{
    public partial class BlogSubmitComment : BaseEntrySublayout
    {
        /// <summary>
        /// Gets or sets the CSS class to set on the message panel for error messages
        /// </summary>
        public string ErrorCssClass
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

        protected void Page_Load(object sender, EventArgs e)
        {
            SuccessCssClass = "wb-successtext";
            ErrorCssClass = "wb-errortext";
            LoadEntry();
        }

        protected virtual void LoadEntry()
        {
            // Comments enabled?
            if (CurrentEntry.DisableComments.Checked)
            {
                if (CommentsPanel != null)
                    CommentsPanel.Visible = false;
            }
            else
            {
                if (ValidationSummaryComments != null)
                    ValidationSummaryComments.HeaderText = Translator.Text("REQUIRED_FIELDS");

                if (buttonSaveComment != null)
                    buttonSaveComment.Text = Translator.Text("POST");
            }
        }

        protected void buttonSaveComment_Click(object sender, EventArgs e)
        {
            if (MessagePanel != null)
            {
                MessagePanel.Visible = false;
            }
            if (Page.IsValid)
            {
                Model.Comment comment = new Model.Comment()
                {
                    AuthorName = txtCommentName != null ? txtCommentName.Text : string.Empty,
                    AuthorEmail = txtCommentEmail != null ? txtCommentEmail.Text : string.Empty,
                    Text = txtCommentText != null ? txtCommentText.Text : string.Empty
                };

                if (txtCommentWebsite != null)
                {
                    string website = txtCommentWebsite.Text;
                    if (!String.IsNullOrEmpty(website))
                    {
                        website = website.Contains("://") ? website : String.Format("//{0}", website);
                    }
                    comment.Fields.Add(Constants.Fields.Website, website);
                }
                comment.Fields.Add(Constants.Fields.IpAddress, Context.Request.UserHostAddress);

                var submissionResult = ManagerFactory.CommentManagerInstance.SubmitComment(Sitecore.Context.Item.ID, comment, Sitecore.Context.Language);
                if (submissionResult.IsNull)
                {
                    SetErrorMessage(Translator.Text("COMMENT_SUBMIT_ERROR"));
                }
                else
                {
                    SetSuccessMessage(Translator.Text("COMMENT_SUBMIT_SUCCESS"));
                    ResetCommentFields();
                }

                //check if added comment is available. if so, send it along with the event
                //won't happen unless publishing and indexing is really fast, but worth a try
                CommentItem newComment = ManagerFactory.CommentManagerInstance.GetEntryComments(Sitecore.Context.Item).Where(item => item.ID == submissionResult).SingleOrDefault();
                Sitecore.Events.Event.RaiseEvent(Constants.Events.UI.COMMENT_ADDED, new object[] { newComment });

                //display javascript to scroll right to the comments list
                CommentScroll.Visible = true;
            }
        }

        /// <summary>
        /// Display an error message on the UI
        /// </summary>
        /// <param name="message">The message to display</param>
        protected virtual void SetErrorMessage(string message)
        {
            if (MessagePanel != null)
            {
                MessagePanel.Visible = true;
                MessagePanel.CssClass = ErrorCssClass;
            }

            if (Message != null)
                Message.Text = message;
        }

        /// <summary>
        /// Display a success message on the UI
        /// </summary>
        /// <param name="message"></param>
        protected virtual void SetSuccessMessage(string message)
        {
            if (MessagePanel != null)
            {
                MessagePanel.Visible = true;
                MessagePanel.CssClass = SuccessCssClass;
            }

            if (Message != null)
                Message.Text = message;
        }

        private void ResetCommentFields()
        {
            if (txtCommentName != null)
                txtCommentName.Text = string.Empty;

            if (txtCommentEmail != null)
                txtCommentEmail.Text = string.Empty;

            if (txtCommentWebsite != null)
                txtCommentWebsite.Text = string.Empty;

            if (txtCommentText != null)
                txtCommentText.Text = string.Empty;
        }
    }
}