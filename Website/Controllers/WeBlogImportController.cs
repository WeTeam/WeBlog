using System;
using System.Web.Http;
using Sitecore.Jobs;
using Sitecore.Modules.WeBlog.Import;
using Sitecore.Services.Core;
using Sitecore.Services.Infrastructure.Web.Http;
using Sitecore.StringExtensions;

namespace Sitecore.Modules.WeBlog.Controllers
{
    [ServicesController]
    public class WordPressImportController : ServicesApiController
    {
        private string JobHandle { get; set; }

        [HttpPut]
        [Authorize]
        public string ImportItems(WordPressImportData data)
        {
            var options = new JobOptions("Creating and importing blog", "WeBlog", Context.Site.Name, this, "ImportBlog", new[] { data });
            var job = JobManager.Start(options);
            job.Status.Total = 0;
            JobHandle = job.Handle.ToString();
            return JobHandle;
        }

        private void ImportBlog(WordPressImportData data)
        {
            var master = Sitecore.Configuration.Factory.GetDatabase("master");
            LogMessage("Reading import item", JobHandle);
            var wpPosts = WpImportManager.Import(data.DataSourceId, data.ImportComments, data.ImportCategories, data.ImportTags);

            var blogParent = master.GetItem(data.ParentId);

            LogMessage("Creating blog", JobHandle);
            var blogRoot = WpImportManager.CreateBlogRoot(blogParent, data.BlogName, data.BlogEmail);
            LogTotal(wpPosts.Count, JobHandle);
            LogMessage("Importing posts", JobHandle);
            WpImportManager.ImportPosts(blogRoot, wpPosts, master, (itemName, count) =>
            {
                LogMessage("Importing entry " + itemName, JobHandle);
                LogProgress(count, JobHandle);
            });
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

        private Job GetJob(string jobHandle)
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
