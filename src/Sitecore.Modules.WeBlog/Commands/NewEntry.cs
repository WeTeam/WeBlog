using System.Collections.Specialized;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Globalization;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.WeBlog.Commands
{
    public class NewEntry : Command
    {
        /// <summary>
        /// The settings to use.
        /// </summary>
        protected IWeBlogSettings Settings = null;

        public NewEntry()
            : this(WeBlogSettings.Instance)
        {
        }

        public NewEntry(IWeBlogSettings settings)
        {
            Settings = settings;
        }

        public override void Execute(CommandContext context)
        {
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                NameValueCollection parameters = new NameValueCollection();
                parameters["currentid"] = item.ID.ToString();
                parameters["database"] = item.Database.Name;
                Context.ClientPage.Start(this, "Run", parameters);
            }
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (args.HasResult)
                {
                    var itemTitle = args.Result;

                    var db = Factory.GetDatabase(args.Parameters["database"]);
                    if (db != null)
                    {
                        var currentItem = db.GetItem(args.Parameters["currentid"]);
                        if (currentItem != null)
                        {
                            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(currentItem);
                            if (currentBlog != null)
                            {
                                var template = new TemplateID(currentBlog.BlogSettings.EntryTemplateID);
                                ItemManager.AddFromTemplate(itemTitle, template, currentBlog);
                            }
                            else
                                Logger.Error("Failed to locate blog root item", this);
                        }
                    }
                }
            }
            else
            {
                var db = Factory.GetDatabase(args.Parameters["database"]);
                if (db == null)
                {
                    return;
                }
                var currentItem = db.GetItem(args.Parameters["currentid"]);
                if (currentItem == null)
                {
                    return;
                }

                var itemIsValid = CanCreateEntry(currentItem);
                if (itemIsValid)
                {
                    Context.ClientPage.ClientResponse.Alert("Please create or select a blog first");
                }
                else
                {
                    SheerResponse.Input("Enter the title of your new entry:", "", Sitecore.Configuration.Settings.ItemNameValidation, Translator.Text("'$Input' is not a valid title."), 100);
                    args.WaitForPostBack(true);
                }
            }
        }

        /// <summary>
        /// Determines whether an entry can be created under the <paramref name="targetItem"/>.
        /// </summary>
        /// <param name="targetItem">The item to check.</param>
        /// <returns>True if the entry should be created, otherwise False.</returns>
        protected virtual bool CanCreateEntry(Item targetItem)
        {
            foreach(var templateId in Settings.BlogTemplateIds)
            {
                if (targetItem.TemplateIsOrBasedOn(templateId))
                    return true;
            }

            foreach (var templateId in Settings.EntryTemplateIds)
            {
                if (targetItem.TemplateIsOrBasedOn(templateId))
                    return true;
            }

            return false;
        }
    }
}
