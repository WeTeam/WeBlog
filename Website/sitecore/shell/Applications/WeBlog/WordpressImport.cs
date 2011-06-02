using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Shell.Applications.Install.Dialogs;
using Sitecore.Shell.Applications.Install;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using System.Text;
using Sitecore.Data.Templates;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;
using Sitecore.Modules.Blog.Import;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.SecurityModel;
using Sitecore.Configuration;
using Sitecore.Web;
using Sitecore.Modules.Blog.Items.Blog;

namespace Sitecore.Modules.Blog.sitecore.shell.Applications.WeBlog
{
    public class WordpressImport : Sitecore.Web.UI.Pages.WizardForm
    {
        #region Fields
        protected Edit WordpressXmlFile;
        protected Groupbox ImportOptionsPane;
        protected DataContext DataContext;
        protected TreeviewEx Treeview;

        protected Literal litSummaryName;
        protected Literal litSummaryEmail;
        protected Literal litSummaryPath;
        protected Literal litSummaryWordpressXML;
        protected Literal litSummaryPosts;
        protected Literal litSummaryCategories;
        protected Literal litSummaryComments;
        protected Literal litSummaryTags;

        protected Edit litSettingsName;
        protected Edit litSettingsEmail;

        protected Checkbox ImportPosts;
        protected Checkbox ImportCategories;
        protected Checkbox ImportComments;
        protected Checkbox ImportTags;

        protected Database db = Factory.GetDatabase("master");
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            ImportOptionsPane.Visible = false;
                        
            this.DataContext.GetFromQueryString();

            base.OnLoad(e);
        }

        protected override void ActivePageChanged(string page, string oldPage)
        {
            Assert.ArgumentNotNull(page, "page");
            Assert.ArgumentNotNull(oldPage, "oldPage");

            switch (page)
            {
                case "Summary":
                    litSummaryName.Text = litSettingsName.Value;
                    litSummaryEmail.Text = litSettingsEmail.Value;
                    litSummaryPath.Text = Treeview.GetSelectedItems().First().Paths.FullPath;

                    litSummaryWordpressXML.Text = WordpressXmlFile.Value;
                    litSummaryCategories.Text = ImportCategories.Checked ? "Yes" : "No";
                    litSummaryComments.Text = ImportComments.Checked ? "Yes" : "No";
                    litSummaryPosts.Text = ImportPosts.Checked ? "Yes" : "No";
                    litSummaryTags.Text = ImportTags.Checked ? "Yes" : "No";
                    break;
                case "CreatingBlog":

                    string fileLocation = string.Format("{0}\\{1}", ApplicationContext.PackagePath, WordpressXmlFile.Value);

                    List<WpPost> listWordpressPosts = WpImportManager.Import(fileLocation, ImportComments.Checked, ImportCategories.Checked, ImportTags.Checked);
                    Item root = db.GetItem(litSummaryPath.Text);

                    BranchItem newBlog = db.Branches.GetMaster(new ID(Sitecore.Configuration.Settings.GetSetting("Blog.BlogBranchTemplateID")));
                    BlogItem blogItem = root.Add(Utilities.Items.MakeSafeItemName(litSettingsName.Value), newBlog);

                    blogItem.BeginEdit();
                    blogItem.Email.Field.Value = litSettingsEmail.Value;
                    blogItem.EndEdit();

                    WpImportManager.ImportPosts(blogItem, listWordpressPosts, db);


                    base.EndWizard();
                    break;
                default:
                    break;
            }
            
            base.ActivePageChanged(page, oldPage);
        }

        protected void OK_Click()
        {
            Item selectionItem = this.Treeview.GetSelectionItem();
            if (selectionItem == null)
            {
                SheerResponse.Alert("Select an item.", new string[0]);
                return;
            }
        }
        
        #region Upload XML
        [HandleMessage("installer:upload", true)]
        protected void Upload(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
		{
			UploadPackageForm.Show(ApplicationContext.PackagePath, true);
			args.WaitForPostBack();
			return;
		}
		if (args.Result.StartsWith("ok:"))
		{
			string str = args.Result.Substring("ok:".Length);

            WordpressXmlFile.Value = str;
		}
        }

        [HandleMessage("installer:browse", true)]
        protected void Browse(ClientPipelineArgs args)
        {
            if (args.IsPostBack &&  args.HasResult)
            { 
                if (WordpressXmlFile != null)
                {
                    WordpressXmlFile.Value = args.Result;
                    ImportOptionsPane.Visible = true;
                    return;
                }
            }
            else
            {
                BrowseDialog.BrowseForOpen(ApplicationContext.PackagePath, "*.xml", "Open XML file", "Select the file that you want to open.", "People/16x16/box.png");
                args.WaitForPostBack();
            }
        }
        #endregion
    }
}