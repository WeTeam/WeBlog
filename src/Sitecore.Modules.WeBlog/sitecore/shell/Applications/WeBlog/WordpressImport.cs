using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecore.Modules.WeBlog.Import;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Import.Providers;
using Sitecore.Shell.Applications.Install;
using Sitecore.Shell.Applications.Install.Dialogs;
using Sitecore.Shell.Framework;
using Sitecore.StringExtensions;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.WeBlog.sitecore.shell.Applications.WeBlog
{
    public class WordpressImport : Sitecore.Web.UI.Pages.WizardForm
    {
        #region Fields
        protected Edit BlogName;
        protected Edit BlogEmail;
        protected Edit WordpressXmlFile;
        protected Groupbox ImportOptionsPane;
        protected Checkbox ImportPosts;
        protected Checkbox ImportCategories;
        protected Checkbox ImportComments;
        protected Checkbox ImportTags;
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
        protected Memo ImportSummary;
        protected Border SuccessMessage;
        protected Border ErrorMessage;

        protected Database db = ContentHelper.GetContentDatabase();
        #endregion

        protected string JobHandle
        {
            get { return StringUtil.GetString(ServerProperties["JobHandle"]); }

            set { ServerProperties["JobHandle"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            ImportOptionsPane.Visible = false;

            this.DataContext.GetFromQueryString();

            base.OnLoad(e);
        }

        protected override bool ActivePageChanging(string page, ref string newpage)
        {
            if (page == "Settings")
            {
                return ValidateSettings();
            }

            return base.ActivePageChanging(page, ref newpage);
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
                    litSummaryName.Text = BlogName.Value;
                    litSummaryEmail.Text = BlogEmail.Value;
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

                ShowResult();
            }
        }

        private void StartImport()
        {
            // Start job for index rebuild
            var options = new JobOptions("Creating and importing blog", "WeBlog", Context.Site.Name, this, "ImportBlog");
            var job = JobManager.Start(options);
            job.Status.Total = 0;
            JobHandle = job.Handle.ToString();

            // Start client pipeline to check progress
            Context.ClientPage.ClientResponse.Timer("CheckStatus", 500);
        }

        private ImportSummary ImportBlog()
        {
            var options = new WpImportOptions
            {
                IncludeComments = ImportComments.Checked,
                IncludeCategories = ImportCategories.Checked,
                IncludeTags = ImportTags.Checked
            };

            string fileLocation = String.Format("{0}\\{1}", ApplicationContext.PackagePath, WordpressXmlFile.Value);
            LogMessage("Reading import file");
            var importManager = new WpImportManager(db, new FileBasedProvider(fileLocation), options);

            LogMessage("Creating blog");
            Item root = db.GetItem(litSummaryPath.Text);
            if (root != null)
            {
                var blogItem = importManager.CreateBlogRoot(root, BlogName.Value, BlogEmail.Value);

                LogMessage("Importing posts");
                LogTotal(importManager.Posts.Count);

                return importManager.ImportPosts(blogItem, (itemName, count) =>
                {
                    LogMessage("Importing entry " + itemName);
                    LogProgress(count);
                });
            }
            else
            {
                LogMessage(String.Format("Parent item for blog root could not be found ({0})", litSummaryPath.Text));
            }

            return null;
        }

        protected void CheckStatus()
        {
            var status = string.Empty;
            var processed = string.Empty;

            var job = GetJob();
            if (job != null)
            {
                if (job.Status.Messages.Count >= 1)
                    status = job.Status.Messages[job.Status.Messages.Count - 1];

                processed = "Processed {0} entries of {1} total".FormatWith(job.Status.Processed,
                                                                                       job.Status.Total);

                if (job.IsDone)
                {
                    Active = "LastPage";
                }
                else
                    Context.ClientPage.ClientResponse.Timer("CheckStatus", 500);
            }

            SheerResponse.SetInnerHtml("StatusMessage", $"{processed}<br/><br/>{status}");
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
            var job = GetJob();
            if (job == null)
                return;

            if (job.Status.Failed)
            {
                SuccessMessage.Visible = false;
                ErrorMessage.Visible = true;

                foreach (var line in job.Status.Messages)
                {
                    ImportSummary.Value += line + "\r\n";
                }
            }
            else
            {
                if (job.Status.Result != null)
                {
                    var summary = job.Status.Result as ImportSummary;
                    ImportSummary.Value =
                        $"Posts created: {summary.PostCount}\r\nCategories created: {summary.CategoryCount}\r\nComments created: {summary.CommentCount}";
                }
            }
        }

        protected virtual bool ValidateSettings()
        {
            if (string.IsNullOrEmpty(BlogName.Value) || string.IsNullOrEmpty(BlogEmail.Value))
            {
                Context.ClientPage.ClientResponse.Alert("Both name and email are required");
                return false;
            }

            return true;
        }

        private Job GetJob()
        {
            if (Context.Job != null)
            {
                return Context.Job;
            }

            var handle = Handle.Parse(JobHandle);
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
            Files.UploadToDirectory(ApplicationContext.PackagePath);
        }

        [HandleMessage("installer:browse", true)]
        protected void Browse(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                BrowseDialog.BrowseForOpen(ApplicationContext.PackagePath, "*.xml", "Open XML file", "Select the file that you want to open.", "People/16x16/box.png");
                args.WaitForPostBack();
            }
            else if (args.HasResult)
            { 
                if (WordpressXmlFile != null)
                {
                    WordpressXmlFile.Value = args.Result;
                    ImportOptionsPane.Visible = true;
                    return;
                }
            }
        }
        #endregion
    }
}