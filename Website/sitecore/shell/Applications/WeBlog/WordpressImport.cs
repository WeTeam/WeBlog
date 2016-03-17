using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecore.Modules.WeBlog.Import;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Shell.Applications.Install;
using Sitecore.Shell.Applications.Install.Dialogs;
using Sitecore.StringExtensions;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;

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
        protected Literal ProgressMessage;
        protected Literal Status;
        protected Memo ResultText;
        protected Border ResultLabel;
        protected Border ShowResultPane;

        protected Edit litSettingsName;
        protected Edit litSettingsEmail;
        protected Edit JobHandle;

        protected Checkbox ImportPosts;
        protected Checkbox ImportCategories;
        protected Checkbox ImportComments;
        protected Checkbox ImportTags;

        protected Database db = ContentHelper.GetContentDatabase();
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

            if (page == "LastPage")
            {
                NextButton.Disabled = true;
                BackButton.Disabled = true;
                CancelButton.Disabled = false;
            }
        }

        private void StartImport()
        {
            // Start job for index rebuild
            var options = new JobOptions("Creating and importing blog", "WeBlog", Context.Site.Name, this, "ImportBlog");
            var job = JobManager.Start(options);
            job.Status.Total = 0;
            JobHandle.Value = job.Handle.ToString();

            // Start client pipeline to check progress
            Context.ClientPage.ClientResponse.Timer("CheckStatus", 500);
        }

        private void ImportBlog()
        {
            LogMessage("Reading import file");
            string fileLocation = string.Format("{0}\\{1}", ApplicationContext.PackagePath, WordpressXmlFile.Value);
            List<WpPost> listWordpressPosts = WpImportManager.Import(fileLocation, ImportComments.Checked, ImportCategories.Checked, ImportTags.Checked);

            LogMessage("Creating blog");


            Item root = db.GetItem(litSummaryPath.Text);
            var blogItem = WpImportManager.CreateBlogRoot(root, litSettingsName.Value, litSettingsEmail.Value);

            LogMessage("Importing posts");
            LogTotal(listWordpressPosts.Count);

            WpImportManager.ImportPosts(blogItem, listWordpressPosts, db, (itemName, count) =>
                                                                              {
                                                                                  LogMessage("Importing entry " + itemName);
                                                                                  LogProgress(count);
                                                                              });
        }

        protected void CheckStatus()
        {
            var job = GetJob();
            if (job != null)
            {
                if (job.Status.Messages.Count >= 1)
                    StatusMessage.Text = job.Status.Messages[job.Status.Messages.Count - 1];

                ProgressMessage.Text = "Processed {0} entries of {1} total".FormatWith(job.Status.Processed,
                                                                                       job.Status.Total);

                if (job.IsDone)
                {
                    if (job.Status.Failed)
                    {
                        Status.Text = "Import failed";

                        foreach (var line in job.Status.Messages)
                        {
                            ResultText.Value += line + "\r\n";
                        }
                    }
                    else
                    {
                        Status.Text = ProgressMessage.Text;
                    }

                    Active = "LastPage";
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

        protected void ShowResult()
        {
            ShowResultPane.Visible = false;
            ResultText.Visible = true;
            ResultLabel.Visible = true;
        }

        private Job GetJob()
        {
            var handle = Handle.Parse(JobHandle.Value);
            if (handle != null)
            {
                return JobManager.GetJob(handle);
            }

            return null;
        }

        private void LogMessage(string message)
        {
            var job = GetJob();
            if (job != null)
                job.Status.Messages.Add(message);
        }

        private void LogProgress(int count)
        {
            var job = GetJob();
            if (job != null)
                job.Status.Processed = count;
        }

        private void LogTotal(int total)
        {
            var job = GetJob();
            if (job != null)
                job.Status.Total = total;
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
            if (args.IsPostBack && args.HasResult)
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