using System;
using System.Linq;
using System.Web.UI.WebControls;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Globalization;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    public partial class BlogSubmitComment : BaseEntrySublayout
    {
        /// <summary>
        /// Gets or sets the CSS class to set on the message panel for error messages
        /// </summary>
        public string ErrorCssClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class to set on the message panel for success messages
        /// </summary>
        public string SuccessCssClass { get; set; }

        public ISubmitCommentCore SubmitCommentCore { get; set; }

        public BlogSubmitComment(ISubmitCommentCore submitCommentCore = null)
        {
            SubmitCommentCore = submitCommentCore ?? new SubmitCommentCore();
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
            if (!CurrentBlog.EnableComments.Checked || CurrentEntry.DisableComments.Checked)
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
                Comment comment = new Comment
                {
                    AuthorName = GetFormValue(txtCommentName),
                    AuthorEmail = GetFormValue(txtCommentEmail),
                    Text = GetFormValue(txtCommentText)
                };
                comment.Fields.Add(Constants.Fields.Website, GetFormValue(txtCommentWebsite));
                comment.Fields.Add(Constants.Fields.IpAddress, Context.Request.UserHostAddress);

                var submissionResult = SubmitCommentCore.Submit(comment);
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
                CommentItem newComment = ManagerFactory.CommentManagerInstance.GetEntryComments(Sitecore.Context.Item).SingleOrDefault(item => item.ID == submissionResult);
                Sitecore.Events.Event.RaiseEvent(Constants.Events.UI.COMMENT_ADDED, new object[] { newComment });

                //display javascript to scroll right to the comments list
                CommentScroll.Visible = true;
            }
        }

        protected virtual string GetFormValue(TextBox textBox)
        {
            return textBox != null ? textBox.Text : string.Empty;
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