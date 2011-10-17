using System;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog.Controls
{
    public partial class AddComment : System.Web.UI.UserControl
    {
        #region Fields
        public Items.WeBlog.BlogHomeItem currentBlog = BlogManager.GetCurrentBlog();
        #endregion

        #region Page methods
        protected void Page_Load(object sender, EventArgs e)
        {
            EntryItem currentEntry = new EntryItem(Sitecore.Context.Item);

            if (!currentEntry.DisableComments.Checked)
            {
                //Create a random integer to prevent caching of captcha image
                Random RandomClass = new Random();
                int randomNumber = RandomClass.Next(1000);
                CaptchaImage.ImageUrl = "~/sitecore modules/Blog/CaptchaHandler.ashx?=" + randomNumber;

                txtAddYourComment.Item = BlogManager.GetCurrentBlog().InnerItem;
            }
            else
            {
                Controls.Clear();
            }
        }

        protected void buttonSaveComment_Click(object sender, EventArgs e)
        {
            if (CaptchaText.Text == Session["CaptchaText"].ToString())
            {
                Model.Comment comment = new Model.Comment()
                {
                    AuthorName = txtCommentName.Text,
                    AuthorEmail = txtCommentEmail.Text,
                    /*AuthorWebsite = txtCommentWebsite.Text,
                    AuthorIP = Context.Request.UserHostAddress,*/
                    Text = txtCommentText.Text
                };
                ID submissionResult = CommentManager.SubmitComment(Sitecore.Context.Item.ID, comment);
                if (submissionResult.IsNull)
                    CaptchaErrorText.Controls.Add(new LiteralControl("<span class='errortext'>An error occurred during comment submission. Please try again later.</span>"));
            }
            else
            {
                CaptchaErrorText.Controls.Add(new LiteralControl("<span class='errortext'>Captcha text is not valid</span>"));
            }
        }
        #endregion
    }
}