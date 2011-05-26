using System;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Web;
using Sitecore.Configuration;
using Sitecore.Modules.Blog.Items.Blog;
using Sitecore.Data.Managers;
using Sitecore.Modules.Blog.Utilities;
using System.Configuration;

namespace Sitecore.Modules.Blog.Admin
{
    public partial class NewBlog : System.Web.UI.Page
    {
        #region Fields
        protected Database db = Factory.GetDatabase(WebUtil.GetQueryString("database"));
        #endregion

        #region Page Methods
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            // Get the properties
            string currentItemID = WebUtil.GetQueryString("id");
            Item parent = db.GetItem(currentItemID);
            string BlogTitle = tbTitle.Text;

            // Create the item based on the branch template
            BranchItem newBlog = db.Branches.GetMaster(new ID("{6FC4278C-E043-458B-9D5D-BBA775A9C386}"));
            Item item = parent.Add(BlogTitle, newBlog);

            // Save ID to hidden textbox to return to created item
            hidCreatedId.Value = item.ID.ToString();

            // Fill in the fields
            SetBlogProperties(item.ID);

            // Register close event
            ClientScript.RegisterStartupScript(this.GetType(), "close", "<script type=\"text/javascript\">onClose();</script>");
        }

        private void SetBlogProperties(ID BlogID)
        {
            try
            {
                // Get current blog as item
                Item blogItem = db.GetItem(BlogID);

                // Convert item to Blog
                Sitecore.Modules.Blog.Items.Blog.BlogItem createdBlog = new Sitecore.Modules.Blog.Items.Blog.BlogItem(blogItem);

                // Fill the blog item with the entered values
                createdBlog.BeginEdit();
                createdBlog.EnableRSS.Field.Checked = chkEnableRSS.Checked;
                createdBlog.ShowEmailWithinComments.Field.Checked = chkShowEmailInComments.Checked;
                createdBlog.EnableLiveWriter.Field.Checked = chkEnableLiveWriter.Checked;
                createdBlog.Theme.Field.Value = "{007B0F91-DE44-404F-9A06-D4AD595F1C52}";
                createdBlog.Email.Field.Value = tbEmail.Text;
                if (tbItemCount.Text != null)
                {
                    createdBlog.DisplayItemCount.Field.Value = tbItemCount.Text;
                }
                if (tbCommentsSidebarCount.Text != null)
                {
                    createdBlog.DisplayCommentSidebarCount.Field.Value = tbCommentsSidebarCount.Text;
                }
                createdBlog.EndEdit();

                Publish.PublishItem(createdBlog.ID, true);

                //Create media library folder
                Item mediaLibrayBlog = db.GetItem("/sitecore/media library/Modules/Blog");
                Item newMediaFolder = ItemManager.AddFromTemplate(tbTitle.Text, TemplateIDs.Folder, mediaLibrayBlog);
                Publish.PublishItem(newMediaFolder);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error during creating blog: {0} Error: {1}, Stacktrace: {2}", BlogID, ex.Message, ex.StackTrace), this);
            }
        }
        #endregion
    }
}