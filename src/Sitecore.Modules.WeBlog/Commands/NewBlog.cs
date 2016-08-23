using System;
using System.Collections.Specialized;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Links;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.WeBlog.Commands
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

                if (currentTID == Settings.BlogTemplateIDString)
                {
                    Context.ClientPage.ClientResponse.Alert("Cannot create a blog within a blog");
                }
                else if (currentTID == Sitecore.Configuration.Settings.GetSetting("Blog.EntryTemplateID"))
                {
                    Context.ClientPage.ClientResponse.Alert("Cannot create a blog within a blogentry");
                }
                else
                {
                    UrlString url = new UrlString("/sitecore modules/Blog/Admin/NewBlog.aspx");
                    url.Append("id", args.Parameters["currentid"]);
                    url.Append("database", args.Parameters["database"]);
                    Context.ClientPage.ClientResponse.ShowModalDialog(url.ToString(), true);
                    args.WaitForPostBack(true);
                }
            }
        }
    }
}