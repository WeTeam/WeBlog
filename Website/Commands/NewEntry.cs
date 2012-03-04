using System.Collections.Specialized;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Modules.WeBlog.Globalization;

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
                    var itemTitle = args.Result;

                    var template = new TemplateID(Settings.EntryTemplateId);
                    var db = Sitecore.Configuration.Factory.GetDatabase(args.Parameters["database"]);
                    if (db != null)
                    {
                        var currentItem = db.GetItem(args.Parameters["currentid"]);
                        if (currentItem != null)
                        {
                            var currentBlog = ManagerFactory.BlogManagerInstance.GetCurrentBlog(currentItem);
                            Item newItem = ItemManager.AddFromTemplate(itemTitle, template, currentBlog);

                            ContentHelper.PublishItem(newItem);
                        }
                    }
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
                    SheerResponse.Input("Enter the title of your new entry:", "", Configuration.Settings.ItemNameValidation, Translator.Text("'$Input' is not a valid title."), 100);
                    args.WaitForPostBack(true);
                }
            }
        }
    }
}
