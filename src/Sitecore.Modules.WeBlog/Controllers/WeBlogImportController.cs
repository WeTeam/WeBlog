using System;
using System.Linq;
using System.Web.Http;
using Sitecore.Abstractions;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Jobs;
using Sitecore.Modules.WeBlog.Import;
using Sitecore.Modules.WeBlog.Import.Providers;
using Sitecore.Modules.WeBlog.Model;
using Sitecore.Services.Core;
using Sitecore.Services.Infrastructure.Web.Http;
using Sitecore.StringExtensions;

#if FEATURE_NVELOCITY
using NVelocity.App;
#endif

namespace Sitecore.Modules.WeBlog.Controllers
{
    [ServicesController]
    public class WordPressImportController : ServicesApiController
    {
        [HttpPut]
        [Authorize]
        public string ImportItems(WordPressImportData data)
        {
            var options = new
#if SC93
                DefaultJobOptions(
#else
                JobOptions(
#endif
                "Creating and importing blog", "WeBlog", Context.Site.Name, this, "ImportBlog", new[] { data });

#if FEATURE_NVELOCITY
            // Init NVelocity before starting the job, in case something in the job uses it (creates items with a workflow that uses the Extended Email Action)
            Velocity.Init();
#endif

            var job = JobManager.Start(options);
            job.Status.Total = 0;
            return job.Handle.ToString();
        }

        private void ImportBlog(WordPressImportData data)
        {
            var db = Factory.GetDatabase(data.DatabaseName);
            var options = BuildWpImportOptions(data);
            var postProvider = new MediaItemBasedProvider(data.DataSourceId, db);
            var importManager = new WpImportManager(db, postProvider, options);
            string jobHandle = Context.Job.Handle.ToString();
            LogMessage("Reading import item", jobHandle);

            var templateMappingItem = db.GetItem(data.TemplateMappingItemId);
            var templatesMapping = new TemplatesMapping(templateMappingItem);

            var defaultLanguageId = new ID("{AF584191-45C9-4201-8740-5409F4CF8BDD}");
            var languageId = data.LanguageItemId ?? defaultLanguageId;
            var languageItem = db.GetItem(languageId);

            Item blogParent = db.GetItem(data.ParentId, LanguageManager.GetLanguage(languageItem.Name));
            if (blogParent != null)
            {
                LogMessage("Creating blog", jobHandle);
                var blogRoot = importManager.CreateBlogRoot(blogParent, data.BlogName, data.BlogEmail, templatesMapping.BlogRootTemplate);

                LogTotal(importManager.Posts.Count, jobHandle);
                LogMessage("Importing posts", jobHandle);
                importManager.ImportPosts(blogRoot, templatesMapping, (itemName, count) =>
                {
                    LogMessage("Importing entry " + itemName, jobHandle);
                    LogProgress(count, jobHandle);
                });
            }
            else
            {
                LogMessage(String.Format("Parent item for blog root could not be found ({0})", data.ParentId), jobHandle);
            }
        }

        private WpImportOptions BuildWpImportOptions(WordPressImportData data)
        {
            var options = new WpImportOptions
            {
                IncludeComments = data.ImportComments,
                IncludeCategories = data.ImportCategories,
                IncludeTags = data.ImportTags
            };
            return options;
        }

        [HttpGet]
        [Authorize]
        public object CheckStatus(string jobHandle)
        {
            var job = GetJob(jobHandle);
            if (job != null)
            {
                string statusMessage = String.Empty;
                if (job.Status.Messages.Count >= 1)
                {
                    statusMessage = job.Status.Messages[job.Status.Messages.Count - 1];
                }
                var progressMessage = "Processed {0} entries of {1} total".FormatWith(job.Status.Processed, job.Status.Total);
                if (job.IsDone)
                {
                    if (job.Status.Failed)
                    {
                        return new
                        {
                            Code = 500,
                            IsDone = job.IsDone,
                            Message = "Import failed",
                            Status = new
                            {
                                job.Status.Processed,
                                job.Status.Total,
                                StatusMessage = statusMessage
                            }
                        };
                    }
                }
                return new
                {
                    Code = 200,
                    IsDone = job.IsDone,
                    Message = progressMessage,
                    Status = new
                    {
                        job.Status.Processed,
                        job.Status.Total,
                        StatusMessage = statusMessage
                    }
                };
            }
            return new
            {
                Code = 404,
                IsDone = true,
                Message = "Cannot find job",
                Status = new
                {
                    Processed = 0,
                    Total = 0
                }
            };
        }

        private void LogMessage(string message, string jobHandle)
        {
            var job = GetJob(jobHandle);
            if (job != null)
            {
                job.Status.Messages.Add(message);
            }
        }

        private void LogProgress(int count, string jobHandle)
        {
            var job = GetJob(jobHandle);
            if (job != null)
            {
                job.Status.Processed = count;
            }
        }

        private void LogTotal(int total, string jobHandle)
        {
            var job = GetJob(jobHandle);
            if (job != null)
            {
                job.Status.Total = total;
            }
        }

        private
#if SC93
            BaseJob
#else
         Job
#endif
            GetJob(string jobHandle)
        {
            var handle = Handle.Parse(jobHandle);
            if (handle != null)
            {
                return JobManager.GetJob(handle);
            }
            return null;
        }
    }
}
