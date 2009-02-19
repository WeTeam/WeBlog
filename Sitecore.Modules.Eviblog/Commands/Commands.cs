using System;
using System.Collections.Specialized;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Managers;
using Sitecore.Configuration;
using Sitecore.Web;
using Sitecore.Modules.Eviblog.Utilities;
using Sitecore.Modules.Eviblog.Managers;
using Sitecore.Links;

namespace Sitecore.Modules.Eviblog.Commands
{
    #region NewBlog

    public class NewBlog : Command
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
                if ((!String.IsNullOrEmpty(args.Result)) && args.Result != "undefined")
                {
                    Context.ClientPage.SendMessage(this, "item:load(id=" + args.Result + ")");
                }
            }
            else
            {
                string currentTID = args.Parameters["tid"];

                if (currentTID == Settings.Default.BlogTemplateID)
                {
                    Context.ClientPage.ClientResponse.Alert("Cannot create a blog within a blog");
                }
                else if (currentTID == Settings.Default.EntryTemplateID)
                {
                    Context.ClientPage.ClientResponse.Alert("Cannot create a blog within a blogentry");
                }
                else
                {
                    UrlString url = new UrlString("/sitecore modules/EviBlog/Admin/NewBlog.aspx");
                    url.Append("id", args.Parameters["currentid"]);
                    url.Append("database", args.Parameters["database"]);
                    Context.ClientPage.ClientResponse.ShowModalDialog(url.ToString(), true);
                    args.WaitForPostBack(true);
                }
            }
        }
    }

    #endregion

    #region NewEntry

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
                    TemplateID template = new TemplateID(new ID(Settings.Default.EntryTemplateID));

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

                if (currentTID != Settings.Default.BlogTemplateID && currentTID != Settings.Default.EntryTemplateID)
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

    #endregion

    #region NewCategory

    public class NewCategory : Command
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
                    TemplateID template = new TemplateID(new ID(Settings.Default.CategoryTemplateID));

                    Item currentItem = current.GetItem(args.Parameters["currentid"]);
                    Item categories = currentItem.Axes.GetChild("Categories");
                    
                    Item newItem = ItemManager.AddFromTemplate(itemTitle, template, categories);

                    Publish.PublishItem(newItem);

                    SheerResponse.Eval("scForm.browser.getParentWindow(scForm.browser.getFrameElement(window).ownerDocument).location.reload(true)");

                    args.WaitForPostBack(true);
                }
            }
            else
            {
                string currentTID = args.Parameters["tid"];

                if (currentTID != Settings.Default.BlogTemplateID && currentTID != Settings.Default.EntryTemplateID)
                {
                    Context.ClientPage.ClientResponse.Alert("Please create or select a blog first");
                }
                else
                {
                    SheerResponse.Input("Enter the name of your new category:", "", Configuration.Settings.ItemNameValidation, Translate.Text("'$Input' is not a valid name."), 100);
                    args.WaitForPostBack(true);
                }
            }
        }
    }

    #endregion
}