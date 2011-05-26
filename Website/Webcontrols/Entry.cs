using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.Blog.Items.Blog;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.Blog.WebControls
{
    public class Entry : System.Web.UI.WebControls.WebControl
    {
        private Item currentEntry = Sitecore.Context.Item;
        private Sitecore.Modules.Blog.Items.Blog.BlogItem currentBlog = BlogManager.GetCurrentBlog();

        private TextBox name = new TextBox();
        private TextBox email = new TextBox();
        private TextBox website = new TextBox();
        private TextBox text = new TextBox();

        protected override void CreateChildControls()
        {
            Controls.Add(new LiteralControl("<div class=\"entry\">"));
            Controls.Add(new LiteralControl("<h2"));
            FieldRenderer entryTitle = new FieldRenderer() { FieldName = "Title", Item = currentEntry };
            Controls.Add(entryTitle);
            Controls.Add(new LiteralControl("</h2>"));
            Controls.Add(new LiteralControl("<div class=\"details\">"));
            Controls.Add(new LiteralControl(string.Format("Posted on: {0}", currentEntry.Statistics.Created.ToString("dddd, MMMM d, yyyy"))));
            Controls.Add(new LiteralControl("</div>"));

            Controls.Add(new LiteralControl("<p>"));
            FieldRenderer entryIntroduction = new FieldRenderer() { FieldName = "Introduction", Item = currentEntry };
            Controls.Add(entryIntroduction);
            Controls.Add(new LiteralControl("</p>"));
            Controls.Add(new LiteralControl("<p>"));
            FieldRenderer entryContent = new FieldRenderer() { FieldName = "Content", Item = currentEntry };
            Controls.Add(entryContent);
            Controls.Add(new LiteralControl("</p>"));
            
            Controls.Add(new LiteralControl("<ul class=\"entry-categories\">"));
            Controls.Add(new LiteralControl("<li>Posted in&nbsp;</li>"));

            foreach (var category in CategoryManager.GetCategoriesByEntryID(currentEntry.ID))
            {
                Controls.Add(new LiteralControl("<li>"));
                HyperLink link = new HyperLink() { NavigateUrl = LinkManager.GetItemUrl(Sitecore.Context.Database.GetItem(category.ID)) };
                FieldRenderer entryCategory = new FieldRenderer() { FieldName = "Title", Item = category.InnerItem };
                link.Controls.Add(entryCategory);
                Controls.Add(link);
                Controls.Add(new LiteralControl("</li>"));
            }
             
            Items.Blog.EntryItem current = new Items.Blog.EntryItem(currentEntry);

            if (!current.DisableComments.Checked)
            {
                BuildAddComments();
                GetComments();
            }

            base.CreateChildControls();
        }

        private void BuildAddComments()
        {
            
            text.TextMode = TextBoxMode.MultiLine;
            text.Rows = 5;

            Label nameLabel = new Label() { Text = "Name" };
            Label emailLabel = new Label() { Text = "Email" };
            Label websiteLabel = new Label() { Text = "Website" };
            Label textLabel = new Label() { Text = "Comment" };

            Button addComment = new Button() { Text = "Add" };
            addComment.Click += new EventHandler(addComment_Click);

            Controls.Add(new LiteralControl("<div class=\"AddComment\">"));
            Controls.Add(nameLabel);
            Controls.Add(name);
            Controls.Add(new LiteralControl("<br />"));
            Controls.Add(emailLabel);
            Controls.Add(email);
            Controls.Add(new LiteralControl("<br />"));
            Controls.Add(websiteLabel);
            Controls.Add(website);
            Controls.Add(new LiteralControl("<br />"));
            Controls.Add(textLabel);
            Controls.Add(text);
            Controls.Add(new LiteralControl("<br />"));
            Controls.Add(addComment);
            Controls.Add(new LiteralControl("</div>"));
        }

        void addComment_Click(object sender, EventArgs e)
        {
            //TODO werkt nog niet
            //CommentManager.AddCommentToEntry(name.Text, email.Text, website.Text, text.Text);
            Model.Comment comment = new Model.Comment()
            {
                AuthorName = name.Text,
                AuthorEmail = email.Text,
                AuthorWebsite = website.Text,
                AuthorIP = Context.Request.UserHostAddress,
                Text = text.Text
            };
            CommentManager.SubmitComment(Sitecore.Context.Item.ID, comment);
        }

        private void GetComments()
        {
            if (CommentManager.GetCommentsCount() != 0)
            {
                foreach (CommentItem item in CommentManager.GetEntryComments())
                {
                    Controls.Add(new LiteralControl("<li>"));
                    HyperLink user = new HyperLink() { NavigateUrl = item.Website.Text, Text = item.Name.Text };
                    Controls.Add(user);
                    Controls.Add(new LiteralControl("<span class=\"comment-email\">"));
                    HyperLink email = new HyperLink() { NavigateUrl = "mailto:" + item.Email, Text = item.Email.Text };
                    Controls.Add(email);
                    Controls.Add(new LiteralControl("</span>"));
                    Controls.Add(new LiteralControl("<div class=\"datetime\">"));
                    Controls.Add(new LiteralControl(item.InnerItem.Statistics.Created.ToString("MMMM d, yyyy")));
                    Controls.Add(new LiteralControl("</div>"));
                    Controls.Add(new LiteralControl("<p>"));
                    Controls.Add(new LiteralControl(item.Comment.Text));
                    Controls.Add(new LiteralControl("</p>"));
                    Controls.Add(new LiteralControl("</li>"));
                }
                
            }
        }

    }
}
