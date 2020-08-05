using System;
using System.Linq;
using System.Web.UI.WebControls;
using Sitecore.Data;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Components;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Globalization;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Model;

namespace Sitecore.Modules.WeBlog.WebForms.Layouts
{
    [AllowDependencyInjection]
    public partial class BlogSubmitComment : BaseEntrySublayout
    {
        /// <summary>
        /// Gets or sets the CSS class to set on the message panel for error messages
        /// </summary>
        protected string ErrorCssClass { get; set; }

        /// <summary>
        /// Gets the CSS class to set on the message panel for success messages
        /// </summary>
        protected string SuccessCssClass { get; set; }

        /// <summary>
        /// Gets the <see cref="ISubmitCommentCore"/> to use to submit a comment.
        /// </summary>
        protected ISubmitCommentCore SubmitCommentCore { get; }

        /// <summary>
        /// Gets the comment manager used to work with comments.
        /// </summary>
        protected ICommentManager CommentManager { get; }

        /// <summary>
        /// Gets the <see cref="IValidateCommentCore"/> to use to validate submitted comments.
        /// </summary>
        protected IValidateCommentCore ValidateCommentCore { get; }

        public BlogSubmitComment(ISubmitCommentCore submitCommentCore, ICommentManager commentManager, IValidateCommentCore validateCommentCore)
        {
            SubmitCommentCore = submitCommentCore;
            CommentManager = commentManager;
            ValidateCommentCore = validateCommentCore;
        }

        public BlogSubmitComment()
        {
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
                var fieldErrorPhrase = Translator.Text(Constants.TranslationPhrases.RequiredField);

                if (ValidationSummaryComments != null)
                    ValidationSummaryComments.HeaderText = Translator.Text("COMMENT_DATA_INVALID");

                if (buttonSaveComment != null)
                    buttonSaveComment.Text = Translator.Text("POST");

                if (rfvCommentName != null)
                {
                    var field = Translator.Text(Constants.TranslationPhrases.Name);
                    var text = string.Format(fieldErrorPhrase, field);
                    rfvCommentName.ErrorMessage = text;
                }

                if(rfvCommentEmail != null)
                {
                    var field = Translator.Text(Constants.TranslationPhrases.Email);
                    var text = string.Format(fieldErrorPhrase, field);
                    rfvCommentEmail.ErrorMessage = text;
                }

                if(rfvCommentText != null)
                {
                    var field = Translator.Text(Constants.TranslationPhrases.Comment);
                    var text = string.Format(fieldErrorPhrase, field);
                    rfvCommentText.ErrorMessage = text;
                }
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

                var result = ValidateCommentCore.Validate(comment, Request.Form);

                ID submissionResult = null;

                if (result.Success)
                {
                    submissionResult = SubmitCommentCore.Submit(comment);
                    if (submissionResult.IsNull)
                    {
                        SetErrorMessage(Translator.Text("COMMENT_SUBMIT_ERROR"));
                    }
                    else
                    {
                        SetSuccessMessage(Translator.Text("COMMENT_SUBMIT_SUCCESS"));
                        ResetCommentFields();
                    }
                }
                else
                {
                    var text = string.Join(", ", result.Errors);
                    SetErrorMessage(text);
                }

                //check if added comment is available. if so, send it along with the event
                //won't happen unless publishing and indexing is really fast, but worth a try
                var newCommentReference = CommentManager.GetEntryComments(Sitecore.Context.Item, int.MaxValue).SingleOrDefault(item => item.Uri.ItemID == submissionResult);
                CommentItem newComment = null;

                if (newCommentReference != null)
                    newComment = Database.GetItem(newCommentReference.Uri);

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