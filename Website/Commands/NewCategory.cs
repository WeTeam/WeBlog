using System.Collections.Specialized;
using Sitecore.Data;
using Sitecore.Data.Managers;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Globalization;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.WeBlog.Commands
{
    public class NewCategory : Command
    {
        public override void Execute(CommandContext context)
        {
            if (context.Items.Length == 1)
            {
                var item = context.Items[0];
                var parameters = new NameValueCollection();
                parameters["currentid"] = item.ID.ToString();
                parameters["database"] = item.Database.Name;
                Context.ClientPage.Start(this, "Run", parameters);
            }
        }

        protected void Run(ClientPipelineArgs args)
        {
            var db = ContentHelper.GetContentDatabase();
            var currentItem = db.GetItem(args.Parameters["currentid"]);

            if (args.IsPostBack)
            {
                if (args.HasResult)
                {
                    string itemTitle = args.Result;

                    var blogItem = ManagerFactory.BlogManagerInstance.GetCurrentBlog(currentItem);

                    if (blogItem == null)
                    {
                        SheerResponse.Alert("Failed to locate the blog item to add the category to.", true);
                        return;
                    }

                    var template = new TemplateID(blogItem.BlogSettings.CategoryTemplateID);
                    var categories = ManagerFactory.CategoryManagerInstance.GetCategoryRoot(currentItem);
                    ItemManager.AddFromTemplate(itemTitle, template, categories);

                    SheerResponse.Eval("scForm.browser.getParentWindow(scForm.browser.getFrameElement(window).ownerDocument).location.reload(true)");

                    args.WaitForPostBack(true);
                }
            }
            else
            {
                if (!currentItem.TemplateIsOrBasedOn(Settings.BlogTemplateID) && !currentItem.TemplateIsOrBasedOn(Settings.EntryTemplateID))
                {
                    Context.ClientPage.ClientResponse.Alert("Please create or select a blog first");
                }
                else
                {
                    SheerResponse.Input("Enter the name of your new category:", "", Configuration.Settings.ItemNameValidation, Translator.Text("'$Input' is not a valid name."), 100);
                    args.WaitForPostBack(true);
                }
            }
        }
    }
}
