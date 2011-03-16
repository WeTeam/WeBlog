using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Links;
using Sitecore.Modules.Eviblog.Items;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Data.Items;
using Microsoft.Security.Application;
using System.Text;

namespace Sitecore.Modules.Eviblog.UserControls
{
    public class BlogEntry : UserControl
    {
        #region Fields

        protected PlaceHolder Message;
        protected Image CaptchaImage;
        protected TextBox CaptchaText;
        protected Panel CommentsPanel;
        public Items.Blog currentBlog = BlogManager.GetCurrentBlog();
        public Items.Entry currentEntry = new Entry(Sitecore.Context.Item);
        protected ListView ListViewCategories;
        protected ListView ListViewComments;
        protected LoginView LoginViewTags;
        protected PlaceHolder PostedDate;
        protected PlaceHolder PostedBy;
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

            // Set creation date and created by of entry
            PostedDate.Controls.Add(new LiteralControl(current.Created.ToString("dddd, MMMM d, yyyy")));
            PostedBy.Controls.Add(new LiteralControl(current.CreatedBy.LocalName));

            // Fill categories
            ListViewCategories.DataSource = CategoryManager.GetCategoriesByEntryID(current.ID);
            ListViewCategories.ItemDataBound += ListViewCategories_ItemDataBound;
            ListViewCategories.DataBind();

            // Update the title of the page
            PlaceHolder phEviblogTitle = (PlaceHolder)Page.FindControl("phEviblogTitle");
            if (phEviblogTitle != null)
            {
                phEviblogTitle.Controls.Clear();
                phEviblogTitle.Controls.Add(new LiteralControl(current.Title + " | " + currentBlog.Title));
            }

            //TODO Create edit possibilities for assigning categories on frontend

            #region Comments

            // Comments enabled?
            if (currentEntry.DisableComments == true)
            {
                ListViewComments.Visible = false;
                CommentsPanel.Visible = false;
            }
            else
            {
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

            Web.UI.WebControls.Text txtEmail = (Web.UI.WebControls.Text)e.Item.FindControl("txtCommentEmail");

            if (currentBlog.ShowEmailWithinComments)
            {   
                txtEmail.DataSource = objComment.ID.ToString();
                txtEmail.Visible = true;
            }
            else
            {
                rfvCommentEmail.Enabled = false;
                txtEmail.Visible = false;
            }

            Literal commentText = e.Item.FindControl("commentText") as Literal;
            if (commentText != null)
                commentText.Text = objComment.CommentText;
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
                bool submissionResult = CommentManager.SubmitComment(Sitecore.Context.Item.ID, comment); ;
                if (!submissionResult)
                    Message.Controls.Add(new LiteralControl("<div class='errortext'>An error occurred during comment submission. Please try again later.</div>"));
                else
                {
                    Message.Controls.Add(new LiteralControl("<div class='successtext'>Thank you for your comment</div>"));
                    ResetCommentFields();
                }
            }
        }

        private void ResetCommentFields()
        {
            txtCommentName.Text = string.Empty;
            txtCommentEmail.Text = string.Empty;
            txtCommentWebsite.Text = string.Empty;
            txtCommentText.Text = string.Empty;
        }

        #endregion
    }
}