using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Commands
{
    class EditEntrySettings : Sitecore.Shell.Applications.WebEdit.Commands.FieldEditorCommand
    {
        /// <summary>
        /// The name of the parameter in <c>ClientPipelineArgs</c> containing 
        /// Sitecore item identification information.
        /// </summary>
        private const string URI = "uri";

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length >= 1)
            {
                ClientPipelineArgs args = new ClientPipelineArgs(context.Parameters);
                args.Parameters.Add("uri", context.Items[0].Uri.ToString());
                if (context.Items[0].TemplateIsOrBasedOn(Settings.EntryTemplateID))
                {
                    Context.ClientPage.Start(this, "StartFieldEditor", args);
                }
                else
                {
                    Context.ClientPage.ClientResponse.Alert("Please select an entry first");
                }
            }

        }

        /// <summary>
        /// Retrieve field editor options controlling the field editor,
        /// including the fields displayed.
        /// </summary>
        /// <param name="args">Pipeline arguments.</param>
        /// <param name="form">Form parameters.</param>
        /// <returns>Field editor options.</returns>
        protected override Sitecore.Shell.Applications.WebEdit.PageEditFieldEditorOptions GetOptions(Sitecore.Web.UI.Sheer.ClientPipelineArgs args,NameValueCollection form)
        {
            Sitecore.Diagnostics.Assert.IsNotNull(args, "args");
            Sitecore.Diagnostics.Assert.IsNotNull(form, "form");
            Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(args.Parameters[URI], URI);
            Sitecore.Data.ItemUri uri = Sitecore.Data.ItemUri.Parse(args.Parameters[URI]);
            Sitecore.Diagnostics.Assert.IsNotNull(uri, URI);
            
            Sitecore.Data.Items.Item item = Sitecore.Data.Database.GetItem(uri);
            Sitecore.Diagnostics.Assert.IsNotNull(item, "item");

            List<Sitecore.Data.FieldDescriptor> fields = new List<Sitecore.Data.FieldDescriptor>();

            try
            {
                string[] fieldNames = new string[] { "Category", "Disable comments" };

                foreach (string fieldName in fieldNames)
                {
                    fields.Add(new Sitecore.Data.FieldDescriptor(item, item.Fields[fieldName].Name));
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(string.Format("Could not initialize blogentry fieldeditor. Error {0}, Stacktrace; {1}", ex.Message, ex.StackTrace), this);
            }

            // Field editor options.
            Sitecore.Shell.Applications.WebEdit.PageEditFieldEditorOptions options = new Sitecore.Shell.Applications.WebEdit.PageEditFieldEditorOptions(form, fields);
            options.PreserveSections = false;
            options.DialogTitle = "Assign categories to the current entry";
            options.Icon = item.Appearance.Icon;
            
            return options;
        }
        
            
    }
}