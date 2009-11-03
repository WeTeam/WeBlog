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
using Sitecore.Shell.Applications.WebEdit.Commands;

namespace Sitecore.Modules.Eviblog.Commands
{
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

                    Item createdBlog = ItemManager.GetItem(new ID(args.Result), Sitecore.Context.Language, Sitecore.Data.Version.Latest, Factory.GetDatabase(args.Parameters["database"]));

                    string oldSiteName = Sitecore.Context.GetSiteName();  
                    Sitecore.Context.SetActiveSite("website");  
                    string url = LinkManager.GetItemUrl(createdBlog);  
                    Sitecore.Context.SetActiveSite(oldSiteName);  

                    SheerResponse.Eval("window.parent.location.href='http://" + WebUtil.GetHostName() + "/" + url + "'");
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
}