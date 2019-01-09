using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Configuration;

namespace Sitecore.Modules.WeBlog.Commands
{
    class EditBlogSettings : Sitecore.Shell.Applications.WebEdit.Commands.FieldEditorCommand
    {
        /// <summary>
        /// The name of the parameter in <c>ClientPipelineArgs</c> containing 
        /// Sitecore item identification information.
        /// </summary>
        private const string URI = "uri";

        protected IWeBlogSettings Settings { get; }

        public EditBlogSettings()
            : this(WeBlogSettings.Instance)
        {
        }

        public EditBlogSettings(IWeBlogSettings settings)
        {
            Settings = settings;
        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length >= 1)
            {
                if (!context.Items[0].TemplateIsOrBasedOn(Settings.BlogTemplateIds))
                    return CommandState.Disabled;
            }

            return base.QueryState(context);
        }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (context.Items.Length >= 1)
            {
                ClientPipelineArgs args = new ClientPipelineArgs(context.Parameters);
                args.Parameters.Add("uri", context.Items[0].Uri.ToString());
                if (context.Items[0].TemplateIsOrBasedOn(Settings.BlogTemplateIds))
                {
                    Context.ClientPage.Start(this, "StartFieldEditor", args);
                }
                else
                {
                    Context.ClientPage.ClientResponse.Alert("Please select an blog first");
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

            while (!item.TemplateIsOrBasedOn(Settings.BlogTemplateIds))
            {
                item = item.Parent;
            }

            Sitecore.Diagnostics.Assert.IsNotNull(item, "item");

            // Fields to display in the field editor.
            List<Sitecore.Data.FieldDescriptor> fields = new List<Sitecore.Data.FieldDescriptor>();
            try
            {
                foreach (string fieldName in GetFieldNames())
                {
                    fields.Add(new Sitecore.Data.FieldDescriptor(item, item.Fields[fieldName].Name));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Could not initialize blogsettings fieldeditor. Error {0}, Stacktrace; {1}", ex.Message, ex.StackTrace), this);
            }

            // Field editor options.
            Sitecore.Shell.Applications.WebEdit.PageEditFieldEditorOptions options = new Sitecore.Shell.Applications.WebEdit.PageEditFieldEditorOptions(form, fields);
            options.PreserveSections = false;
            options.DialogTitle = "Blog settings";
            options.Icon = item.Appearance.Icon;
            return options;
        }

        /// <summary>
        /// Gets the field names to display on the field editor
        /// </summary>
        /// <returns>The names of the fields to display on the field editor</returns>
        protected virtual IEnumerable<string> GetFieldNames()
        {
            yield return "Title";
            yield return "Email";
            yield return "Theme";
            yield return "DisplayItemCount";
            yield return "DisplayCommentSidebarCount";
            yield return "Maximum Entry Image Size";
            yield return "Maximum Thumbnail Image Size";
            yield return "Enable RSS";
            yield return "Enable Comments";
            yield return "Show Email Within Comments";
            yield return "EnableLiveWriter";
            yield return "Enable Gravatar";
            yield return "Gravatar Size";
            yield return "Default Gravatar Style";
            yield return "Gravatar Rating";
            yield return "Custom Dictionary Folder";
        }
    }
}