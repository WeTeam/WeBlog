using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Sitecore.Data.Items;
using Sitecore.Modules.Blog.Managers;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data;
using System.Configuration;

namespace Sitecore.Modules.Blog.Commands
{
    class EditBlogSettings : Sitecore.Shell.Applications.WebEdit.Commands.FieldEditorCommand
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
                if (context.Items[0].TemplateID == new ID(Sitecore.Configuration.Settings.GetSetting("Blog.BlogTemplateID")))
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

            while (item.TemplateID.ToString() != Sitecore.Configuration.Settings.GetSetting("Blog.BlogTemplateID"))
            {
                item = item.Parent;
            }

            Sitecore.Diagnostics.Assert.IsNotNull(item, "item");

            // Fields to display in the field editor.
            List<Sitecore.Data.FieldDescriptor> fields = new List<Sitecore.Data.FieldDescriptor>();
            try
            {
                string[] fieldNames = new string[] { "Title", "Email", "Theme", "DisplayItemCount", "DisplayCommentSidebarCount", "Enable RSS",
                "Enable Comments", "Show Email Within Comments", "EnableLiveWriter" };

                foreach (string fieldName in fieldNames)
                {
                    fields.Add(new Sitecore.Data.FieldDescriptor(item, item.Fields[fieldName].Name));
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(string.Format("Could not initialize blogsettings fieldeditor. Error {0}, Stacktrace; {1}", ex.Message, ex.StackTrace), this);
            }

            // Field editor options.
            Sitecore.Shell.Applications.WebEdit.PageEditFieldEditorOptions options = new Sitecore.Shell.Applications.WebEdit.PageEditFieldEditorOptions(form, fields);
            options.PreserveSections = false;
            options.DialogTitle = "Blog settings";
            options.Icon = item.Appearance.Icon;
            return options;
        }
    }
}