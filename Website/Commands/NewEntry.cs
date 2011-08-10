using System.Collections.Specialized;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Modules.WeBlog.Utilities;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.WeBlog.Commands
{
    public class NewEntry : Command
    {
        public override void Execute(CommandContext context)
        {
            if (context.Items.Length == 1)
            {
                Item item = context.Items[0];
                NameValueCollection parameters = new NameValueCollection();
                parameters["currentid"] = item.ID.ToString();
                parameters["tid"] = item.TemplateID.ToString();
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
                    string itemTitle = args.Result;

                    Database current = Factory.GetDatabase("master");
                    TemplateID template = new TemplateID(Settings.EntryTemplateId);

                    Item currentBlog = BlogManager.GetCurrentBlogItem(new ID(args.Parameters["currentid"]), args.Parameters["database"]);
                    Item newItem = ItemManager.AddFromTemplate(itemTitle, template, currentBlog);
                    newItem.Editing.BeginEdit();
                    newItem.Fields["Introduction"].Value = "Enter the introduction text";
                    newItem.Fields["Content"].Value = "Enter the full article";
                    newItem.Editing.EndEdit();

                    Publish.PublishItem(newItem);
                }
            }
            else
            {
                string currentTID = args.Parameters["tid"];

                if (currentTID != Settings.BlogTemplateIdString && currentTID != Settings.EntryTemplateIdString && currentTID != Settings.EntryTemplateIdString)
                {
                    Context.ClientPage.ClientResponse.Alert("Please create or select a blog first");
                }
                else
                {
                    SheerResponse.Input("Enter the title of your new entry:", "", Configuration.Settings.ItemNameValidation, Translate.Text("'$Input' is not a valid title."), 100);
                    args.WaitForPostBack(true);
                }
            }
        }
    }
}
