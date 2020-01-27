using Scriban;
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Items;
using Sitecore.Modules.WeBlog.Pipelines.PopulateScribanMailActionModel;
using Sitecore.Workflows.Simple;
using System;
using System.Collections.Generic;
using System.Net.Mail;

#if SC82
using Sitecore.Analytics.Commons;
#else
using Sitecore.Analytics.Core;
#endif

namespace Sitecore.Modules.WeBlog.Workflow
{
    public class ScribanMailAction
    {
        /// <summary>
        /// The name of the pipeline used to populate the model.
        /// </summary>
        protected readonly string PopulateModelPipelineName = "weblogPopulateScribanMailActionModel";

        /// <summary>
        /// Gets the <see cref="BaseCorePipelineManager"/> used to execute pipelines.
        /// </summary>
        protected BaseCorePipelineManager PipelineManager { get; }

        /// <summary>
        /// Gets the factory used to create the the <see cref="ISmtpClient"/> instance used to send email messages.
        /// </summary>
        protected Func<ISmtpClient> SmtpClientFactory { get; }

        /// <summary>
        /// Gets the <see cref="BaseLog"/> to write log messages to.
        /// </summary>
        protected BaseLog Log { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public ScribanMailAction()
            : this(ServiceLocator.ServiceProvider.GetService(typeof(BaseCorePipelineManager)) as BaseCorePipelineManager, () => new SmtpClientWrapper(), new DefaultLog())
        {
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="pipelineManager">The <see cref="BaseCorePipelineManager"/> used to execute pipelines.</param>
        /// <param name="smtpClient">The <see cref="ISmtpClient"/> instance used to send email messages.</param>
        /// <param name="log">The <see cref="BaseLog"/> to write log messages to.</param>
        ///public ScribanMailAction(IWeBlogSettings weBlogSettings, IEntryManager entryManager, IBlogManager blogManager, Func<ISmtpClient> smtpClientFactory, BaseLog log)
        public ScribanMailAction(BaseCorePipelineManager pipelineManager, Func<ISmtpClient> smtpClientFactory, BaseLog log)
        {
            if (pipelineManager == null)
                throw new ArgumentNullException(nameof(pipelineManager));

            if (smtpClientFactory == null)
                throw new ArgumentNullException(nameof(smtpClientFactory));

            if (log == null)
                throw new ArgumentNullException(nameof(log));

            PipelineManager = pipelineManager;
            SmtpClientFactory = smtpClientFactory;
            Log = log;
        }

        public virtual void Process(WorkflowPipelineArgs args)
        {
            ScribanMailActionItem actionItem = args.ProcessorItem.InnerItem;
            var mailTo = actionItem.To;

            if (string.IsNullOrWhiteSpace(mailTo))
            {
                Log.Error(GetType().FullName + " cannot be invoked with an empty 'To' field.", this);
                return;
            }

            var mailFrom = actionItem.From;

            if (string.IsNullOrWhiteSpace(mailFrom))
            {
                Log.Error(GetType().FullName + " cannot be invoked with an empty 'From' field.", this);
                return;
            }

            var mailSubject = actionItem.Subject;
            var mailBody = actionItem.Message;

            var model = CreateModel(args);

            try
            {
                mailTo = ProcessScribanTemplate(mailTo, model);
                mailFrom = ProcessScribanTemplate(mailFrom, model);
                mailSubject = ProcessScribanTemplate(mailSubject, model);
                mailBody = ProcessScribanTemplate(mailBody, model);
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred whilst rendering a Scriban template in " + GetType().FullName, ex, this);
                return;
            }

            var message = new MailMessage(mailFrom, mailTo, mailSubject, mailBody);
            message.IsBodyHtml = true;

            try
            {
#if SC82
                var smtpClient = SmtpClientFactory.Invoke();
#else
                using (var smtpClient = SmtpClientFactory.Invoke())
#endif
                {
                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception while sending workflow email", ex, this);
            }
        }

        protected virtual Dictionary<string, object> CreateModel(WorkflowPipelineArgs args)
        {
            var modelArgs = new PopulateScribanMailActionModelArgs(args);
            PipelineManager.Run(PopulateModelPipelineName, modelArgs);
            return modelArgs.GetModel();
        }

        protected string ProcessScribanTemplate(string template, Dictionary<string, object> model)
        {
            var parsedTemplate = Template.Parse(template);

            if(parsedTemplate.HasErrors)
            {
                Log.Error("An error occurred whilst parsing a Scriban template in " + GetType().FullName, this);

                foreach(var message in parsedTemplate.Messages)
                {
                    Log.Error(message.ToString(), this);
                }

                throw new InvalidOperationException("Failed to parse Scriban template");
            }

            return parsedTemplate.Render(model);
        }
    }
}