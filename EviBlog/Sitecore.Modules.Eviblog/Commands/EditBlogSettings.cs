using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Sitecore.Data.Items;
using Sitecore.Modules.Eviblog.Managers;

namespace Sitecore.Modules.Eviblog.Commands
{
    class EditBlogSettings : Sitecore.Shell.Applications.WebEdit.Commands.FieldEditorCommand
    {
        /// <summary>
        /// The name of the parameter in <c>ClientPipelineArgs</c> containing 
        /// Sitecore item identification information.
        /// </summary>
        private const string URI = "uri";

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

            while (item.TemplateID.ToString() != Settings.Default.BlogTemplateID)
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

            // The default single section:
            //options.Title = "Field Editor : " + item.Paths.ContentPath;
            //// or (instead of setting Title): 
            options.PreserveSections = true;
            options.DialogTitle = "Blog settings";
            options.Icon = item.Appearance.Icon;
            return options;
        }
    }
}