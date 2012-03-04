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
using Sitecore.Modules.WeBlog.Import;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.SecurityModel;
using Sitecore.Configuration;
using Sitecore.Web;
using Sitecore.Modules.WeBlog.Items.WeBlog;
using Sitecore.Jobs;

namespace Sitecore.Modules.WeBlog.sitecore.shell.Applications.WeBlog
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
        protected Literal StatusMessage;
        protected Literal Status;

        protected Edit litSettingsName;
        protected Edit litSettingsEmail;
        protected Edit JobHandle;

        protected Checkbox ImportPosts;
        protected Checkbox ImportCategories;
        protected Checkbox ImportComments;
        protected Checkbox ImportTags;

        protected Database db = ContentHelper.GetContentDatabase();
        #endregion

        private string jobStatus;

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

            base.ActivePageChanged(page, oldPage);

            if (page == "Import")
            {
                if (!string.IsNullOrEmpty(WordpressXmlFile.Value))
                {
                    ImportOptionsPane.Visible = true;
                    return;
                }
            }
            if (page == "Summary")
            {
                    litSummaryName.Text = litSettingsName.Value;
                    litSummaryEmail.Text = litSettingsEmail.Value;
                    litSummaryPath.Text = Treeview.GetSelectedItems().First().Paths.FullPath;

                    litSummaryWordpressXML.Text = WordpressXmlFile.Value;
                    litSummaryCategories.Text = ImportCategories.Checked ? "Yes" : "No";
                    litSummaryComments.Text = ImportComments.Checked ? "Yes" : "No";
                    litSummaryPosts.Text = ImportPosts.Checked ? "Yes" : "No";
                    litSummaryTags.Text = ImportTags.Checked ? "Yes" : "No";
            }
            NextButton.Header = "Next >";

            if (page == "Summary")
            {
                NextButton.Header = "Start Import";
            }

            if (page == "ImportJob")
            {
                BackButton.Disabled = true;
                NextButton.Disabled = true;
                StartImport();
            }
        }

        private void StartImport()
        {
            // Start job for index rebuild
            var options = new JobOptions("Creating and importing blog", "WeBlog", Context.Site.Name, this, "ImportBlog");
            var job = JobManager.Start(options);
            JobHandle.Value = job.Handle.ToString();

            // Start client pipeline to check progress
            Context.ClientPage.ClientResponse.Timer("CheckStatus", 500);
        }

        private void ImportBlog()
        {
            jobStatus = "Reading import file";
            string fileLocation = string.Format("{0}\\{1}", ApplicationContext.PackagePath, WordpressXmlFile.Value);
            List<WpPost> listWordpressPosts = WpImportManager.Import(fileLocation, ImportComments.Checked, ImportCategories.Checked, ImportTags.Checked);

            jobStatus = "Creating blog";
            Item root = db.GetItem(litSummaryPath.Text);

            BranchItem newBlog = db.Branches.GetMaster(Settings.BlogBranchId);
            BlogHomeItem blogItem = root.Add(ItemUtil.ProposeValidItemName(litSettingsName.Value), newBlog);

            blogItem.BeginEdit();
            blogItem.Email.Field.Value = litSettingsEmail.Value;
            blogItem.EndEdit();

            jobStatus = "Importing posts";
            WpImportManager.ImportPosts(blogItem, listWordpressPosts, db);

            this.NextButton.Disabled = true;
            this.BackButton.Disabled = true;
            this.CancelButton.Disabled = true;
        }

        protected void CheckStatus()
        {
            var handle = Handle.Parse(JobHandle.Value);
            if (handle != null)
            {
                var job = JobManager.GetJob(handle);
                if (job != null && jobStatus != null)
                {
                    StatusMessage.Text = jobStatus;
                }

                if (job.IsDone)
                {
                    Active = "LastPage";
                    Status.Text = StatusMessage.Text;
                }
                else
                    Context.ClientPage.ClientResponse.Timer("CheckStatus", 500);
            }
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