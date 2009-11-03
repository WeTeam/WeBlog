using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogEntry : UserControl
    {
        #region Fields

        protected PlaceHolder CaptchaErrorText;
        protected Image CaptchaImage;
        protected TextBox CaptchaText;
        protected Panel CommentsPanel;
        public Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        protected ListView ListViewCategories;
        protected ListView ListViewComments;
        protected LoginView LoginViewTags;
        protected PlaceHolder PostedDate;
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
            LoadEntry();
        }

        /// <summary>
        /// Loads the entry.
        /// </summary>
        private void LoadEntry()
        {
            // Create entry of current item
            Entry current = new Entry(Sitecore.Context.Item);

            // Set creation date of entry
            PostedDate.Controls.Add(new LiteralControl(current.Created.ToString("dddd, MMMM d, yyyy")));

            // Fill categories
            ListViewCategories.DataSource = CategoryManager.GetCategoriesByEntryID(current.ID);
            ListViewCategories.ItemDataBound += ListViewCategories_ItemDataBound;
            ListViewCategories.DataBind();

            //TODO Create edit possibilities for assigning categories on frontend

            #region Comments

            // Comments enabled?
            if (currentBlog.EnableComments == false)
            {
                ListViewComments.Visible = false;
                CommentsPanel.Visible = false;
            }
            else
            {
                //Create a random integer to prevent caching of captcha image
                Random RandomClass = new Random();
                int randomNumber = RandomClass.Next(1000);
                CaptchaImage.ImageUrl = "~/sitecore modules/EviBlog/CaptchaHandler.ashx?=" + randomNumber;

                txtAddYourComment.Item = BlogManager.GetCurrentBlogItem();
            }

            // Check for the existence of comments
            if (CommentManager.GetCommentsCount() != 0)
            {
                ListViewComments.DataSource = CommentManager.GetEntryComments();
                ListViewComments.ItemDataBound += ListViewComments_ItemDataBound;
                ListViewComments.DataBind();

                Web.UI.WebControls.Text titleComment =
                    (Web.UI.WebControls.Text) ListViewComments.FindControl("titleComments");
                titleComment.Item = BlogManager.GetCurrentBlogItem();
            }

            #endregion

            #region Tags

            // Get all tags
            var tags = TagManager.GetTagsByEntry(current);

            if (Sitecore.Context.PageMode.IsNormal && !Sitecore.Context.IsLoggedIn)
            {
                foreach (string tag in tags.Keys)
                {
                    HyperLink tagLink = new HyperLink();
                    tagLink.Text = tag;
                    tagLink.NavigateUrl = LinkManager.GetItemUrl(Sitecore.Context.Item.Parent) + "?tag=" + tag;

                    PlaceHolder tagsPlaceholder = (PlaceHolder) LoginViewTags.FindControl("TagsPlaceholder");
                    tagsPlaceholder.Controls.Add(tagLink);
                }
            }

            #endregion
        }

        #endregion

        #region Eventhandlers

        private void ListViewComments_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            Comment objComment = (Comment) ((ListViewDataItem) e.Item).DataItem;

            HyperLink userLink = (HyperLink) e.Item.FindControl("hyperlinkUsername");
            userLink.NavigateUrl = objComment.Website;
            userLink.Text = objComment.UserName;

            Literal litDate = (Literal) e.Item.FindControl("LiteralDate");
            litDate.Text = objComment.Created.ToShortDateString() + " " + objComment.Created.ToShortTimeString();

            if (currentBlog.ShowEmailWithinComments)
            {
                Web.UI.WebControls.Text txtEmail = (Web.UI.WebControls.Text) e.Item.FindControl("txtCommentEmail");
                txtEmail.DataSource = objComment.ID.ToString();
                txtEmail.Visible = true;
            }
            else
            {
                Web.UI.WebControls.Text txtEmail = (Web.UI.WebControls.Text) e.Item.FindControl("txtCommentEmail");
                rfvCommentEmail.Enabled = false;
                txtEmail.Visible = false;
            }

            Web.UI.WebControls.Text txtComment = (Web.UI.WebControls.Text) e.Item.FindControl("txtCommentText");
            txtComment.DataSource = objComment.ID.ToString();
        }

        private void ListViewCategories_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            Category objCategory = (Category) ((ListViewDataItem) e.Item).DataItem;

            Web.UI.WebControls.Text txt = (Web.UI.WebControls.Text) e.Item.FindControl("txtCategorie");
            txt.DataSource = objCategory.ID.ToString();

            HyperLink postLink = (HyperLink) e.Item.FindControl("hyperlinkCategory");
            postLink.NavigateUrl = LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(objCategory.ID));
        }

        protected void buttonSaveComment_Click(object sender, EventArgs e)
        {
            if (CaptchaText.Text == Session["CaptchaText"].ToString())
            {
                string UserName = txtCommentName.Text;
                string Email = txtCommentEmail.Text;
                string Website = txtCommentWebsite.Text;
                string Comment = txtCommentText.Text;

                CommentManager.AddCommentToEntry(UserName, Email, Website, Comment);
            }
            else
            {
                CaptchaErrorText.Controls.Add(new LiteralControl("<span class='errortext'>Captcha text is not valid</span>"));
            }
        }

        #endregion
    }
}