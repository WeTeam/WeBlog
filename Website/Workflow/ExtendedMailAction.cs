/* *********************************************************************** *
 * File   : MailAction.cs                                 Part of Sitecore *
 * Version: 1.00                                          www.sitecore.net *
 * Author : Sitecore A/S                                                   *
 *                                                                         *
 * Purpose: To implement extended mail action                              *
 *                                                                         *
 * Bugs   : None known.                                                    *
 *                                                                         *
 * Status : Published.                                                     *
 *                                                                         *
 * Copyright (C) 1999-2004 by Sitecore A/S. All rights reserved.           *
 *                                                                         *
 * This work is the property of:                                           *
 *                                                                         *
 *        Sitecore A/S                                                     *
 *        Meldahlsgade 5, 4.                                               *
 *        DK-1613 Copenhagen V.                                            *
 *        Denmark                                                          *
 *                                                                         *
 * This is a Sitecore published work under Sitecore's                      *
 * shared source license.                                                  *
 *                                                                         *
 * *********************************************************************** */

#region using

using System;
using System.IO;
using NVelocity;
using NVelocity.App;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Workflows.Simple;
using System.Net.Mail;
using Sitecore.Modules.WeBlog;
using Sitecore.Security.Accounts;
using Sitecore.Security;
using Sitecore.Modules.WeBlog.Managers;
using System.Web.Security;

#endregion

namespace Sitecore.Modules.WeBlog.Workflow
{
    /// <summary>
    /// Uses NVelocity to parse templates in Sitecore mail action
    /// </summary>
    public class ExtendedMailAction
    {
        protected VelocityContext velocityContext;

        public ExtendedMailAction()
        {
            Velocity.Init();
        }

        /// <summary>
        /// Processes the mail action, creates and sends the email
        /// </summary>
        public virtual void Process(WorkflowPipelineArgs args)
        {
            CreateContext(args);
            Item item = args.ProcessorItem.InnerItem;
            string to = ProcessFieldValue("To", item);
            string from = ProcessFieldValue("From", item);
            string subject = ProcessFieldValue("Subject", item);
            string body = ProcessFieldValue("Message", item);

            if (string.IsNullOrEmpty(to))
            {
                Sitecore.Diagnostics.Log.Error("No 'to' email available for Extended Email Action", this);
                return;
            }

            if (string.IsNullOrEmpty(from))
            {
                Sitecore.Diagnostics.Log.Error("No 'from' email available for Extended Email Action", this);
                return;
            }

            MailMessage message = new MailMessage(from, to, subject, body);
            string server = item["mail server"];
            if (string.IsNullOrEmpty(server))
            {
                Sitecore.MainUtil.SendMail(message);
            }
            else
            {
                SmtpClient smtp = new SmtpClient(server);
                smtp.Send(message);
            }
        }

        /// <summary>
        /// Creates velocity context.
        /// </summary>
        /// <remarks>To add your own data to the context, you should
        /// override the <c>PopulateContext</c> method</remarks>
        protected virtual void CreateContext(WorkflowPipelineArgs args)
        {
            velocityContext = new VelocityContext();
            PopulateContext(args);
        }

        /// <summary>
        /// Populates the velocity template context. Only the objects that were
        /// added in this method will be accessible in the mail template.
        /// </summary>
        /// <remarks>Override this to add your own data to the context</remarks>
        protected virtual void PopulateContext(WorkflowPipelineArgs args)
        {
            velocityContext.Put("args", args);
            velocityContext.Put("item", args.DataItem);
            velocityContext.Put("processor", args.ProcessorItem);
            velocityContext.Put("user", Sitecore.Context.User);
            velocityContext.Put("history", args.DataItem.State.GetWorkflow().GetHistory(args.DataItem));
            velocityContext.Put("state", args.DataItem.State.GetWorkflowState());
            velocityContext.Put("nextState", GetNextState(args));
            velocityContext.Put("site", Sitecore.Context.Site);
            velocityContext.Put("time", DateTime.Now);

            Items.WeBlog.EntryItem entryItem = null;
            if (Utilities.Items.TemplateIsOrBasedOn(args.DataItem, Settings.EntryTemplateId))
            {
                entryItem = new Items.WeBlog.EntryItem(args.DataItem);
            }
            else if (Utilities.Items.TemplateIsOrBasedOn(args.DataItem, Settings.CommentTemplateId))
            {
                Items.WeBlog.CommentItem commentItem = new Items.WeBlog.CommentItem(args.DataItem);
                entryItem = EntryManager.GetBlogEntryByComment(commentItem);
                velocityContext.Put("comment", commentItem);
            }

            if (entryItem != null)
            {
                velocityContext.Put("entry", entryItem);
                if (!string.IsNullOrEmpty(entryItem.InnerItem.Statistics.CreatedBy))
                {
                    UserProfile createdBy = User.FromName(entryItem.InnerItem.Statistics.CreatedBy, false).Profile;
                    velocityContext.Put("entryCreatedBy", createdBy);
                }
                if (!string.IsNullOrEmpty(entryItem.InnerItem.Statistics.UpdatedBy))
                {
                    UserProfile updatedBy = User.FromName(entryItem.InnerItem.Statistics.UpdatedBy, false).Profile;
                    velocityContext.Put("entryUpdatedBy", updatedBy);
                }

                Items.WeBlog.BlogHomeItem blog = BlogManager.GetCurrentBlog(entryItem);
                velocityContext.Put("blog", blog);
            }
        }

        /// <summary>
        /// Processes the template, expanding all known values
        /// </summary>
        /// <param name="value">Template to process</param>
        /// <returns>Rendered template</returns>
        protected virtual string ProcessValue(string value, Item item)
        {
            StringWriter result = new StringWriter();
            try
            {
                Velocity.Evaluate(velocityContext, result, "Extended mail action", value);
            }
            catch (NVelocity.Exception.ParseErrorException ex)
            {
                Log.Error(string.Format("Error parsing template for the {0} item \n {1}",
                   item.Paths.Path, ex.ToString()), this);
            }
            return result.GetStringBuilder().ToString();
        }

        #region helpers

        private string GetNextState(WorkflowPipelineArgs args)
        {
            Item command = args.ProcessorItem.InnerItem.Parent;
            string nextStateID = command["Next State"];
            if (nextStateID.Length == 0)
            {
                return string.Empty;
            }

            Item nextState = args.DataItem.Database.Items[ID.Parse(nextStateID)];
            if (nextState != null)
            {
                return nextState.Name;
            }
            return string.Empty;
        }

        private string ProcessFieldValue(string fieldName, Item item)
        {
            string value = item[fieldName];
            if (value.Length > 0)
            {
                return ProcessValue(value, item);
            }
            return value;
        }

        #endregion
    }
}