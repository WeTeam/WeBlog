using System;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Managers;
using System.Web.UI.WebControls;
using Sitecore.Web;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Modules.Eviblog.Items;
using System.Web.UI;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class AddComment : System.Web.UI.UserControl
    {
        #region Fields
        protected PlaceHolder Message;
        protected Image CaptchaImage;
        protected TextBox CaptchaText;
        public Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        protected RequiredFieldValidator rfvCommentEmail;
        protected Web.UI.WebControls.Text titleComments;
        protected Web.UI.WebControls.Text txtAddYourComment;
        protected TextBox txtCommentEmail;
        protected TextBox txtCommentName;
        protected TextBox txtCommentText;
        protected TextBox txtCommentWebsite;
        #endregion

        #region Page methods
        protected void Page_Load(object sender, EventArgs e)
        {
            Entry currentEntry = new Entry(Sitecore.Context.Item);

            if (!currentEntry.DisableComments)
            {
                //Create a random integer to prevent caching of captcha image
                Random RandomClass = new Random();
                int randomNumber = RandomClass.Next(1000);
                CaptchaImage.ImageUrl = "~/sitecore modules/EviBlog/CaptchaHandler.ashx?=" + randomNumber;

                txtAddYourComment.Item = BlogManager.GetCurrentBlogItem();
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
                    AuthorWebsite = txtCommentWebsite.Text,
                    AuthorIP = Context.Request.UserHostAddress,
                    Text = txtCommentText.Text
                };
                bool submissionResult = CommentManager.SubmitComment(Sitecore.Context.Item.ID, comment);
                if (!submissionResult)
                    Message.Controls.Add(new LiteralControl("<span class='errortext'>An error occurred during comment submission. Please try again later.</span>"));
            }
            else
            {
                Message.Controls.Add(new LiteralControl("<span class='errortext'>Captcha text is not valid</span>"));
            }
        }
        #endregion
    }
}