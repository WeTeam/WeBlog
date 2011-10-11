using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Utilities;

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
            SuccessCssClass = "successtext";
            ErrorCssClass = "errortext";
            SublayoutParamHelper helper = new SublayoutParamHelper(this, true);
            LoadEntry();
        }

        protected virtual void LoadEntry()
        {
            // Comments enabled?
            if (CurrentEntry.DisableComments.Checked)
            {
                if(CommentsPanel != null)
                    CommentsPanel.Visible = false;
            }
            else
            {
                if(ValidationSummaryComments != null)
				    ValidationSummaryComments.HeaderText = Translate.Text("REQUIRED_FIELDS");

                if(buttonSaveComment != null)
				    buttonSaveComment.Text = Translate.Text("POST");
            }
        }

        protected void buttonSaveComment_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Model.Comment comment = new Model.Comment()
                {
                    AuthorName = txtCommentName != null ? txtCommentName.Text : string.Empty,
                    AuthorEmail = txtCommentEmail != null ? txtCommentEmail.Text : string.Empty,
                    Text = txtCommentText != null ? txtCommentText.Text : string.Empty
                };

                if(txtCommentWebsite != null)
                    comment.Fields.Add("Website", txtCommentWebsite.Text);
                comment.Fields.Add("IP Address", Context.Request.UserHostAddress);

                var submissionResult = CommentManager.SubmitComment(Sitecore.Context.Item.ID, comment);
                if (submissionResult.IsNull)
                    SetErrorMessage(Translate.Text("COMMENT_SUBMIT_ERROR"));
                else
                {
                    SetSuccessMessage(Translate.Text("COMMENT_SUBMIT_SUCCESS"));
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
            if(MessagePanel != null)
                MessagePanel.CssClass = ErrorCssClass;

            if(Message != null)
                Message.Text = message;
        }

        /// <summary>
        /// Display a success message on the UI
        /// </summary>
        /// <param name="message"></param>
        protected virtual void SetSuccessMessage(string message)
        {
            if(MessagePanel != null)
                MessagePanel.CssClass = SuccessCssClass;

            if(Message != null)
                Message.Text = message;
        }

        private void ResetCommentFields()
        {
            if(txtCommentName != null)
                txtCommentName.Text = string.Empty;

            if(txtCommentEmail != null)
                txtCommentEmail.Text = string.Empty;

            if(txtCommentWebsite != null)
                txtCommentWebsite.Text = string.Empty;

            if(txtCommentText != null)
                txtCommentText.Text = string.Empty;
        }
    }
}