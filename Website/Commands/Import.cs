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
using Sitecore.Modules.WeBlog.Utilities;
using Sitecore.Modules.WeBlog.Managers;
using Sitecore.Links;
using Sitecore.Shell.Applications.WebEdit.Commands;
using System.Configuration;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.WeBlog.Commands
{
    public class Import : Command
    {
        
	    public override void Execute(CommandContext context)
	    {
		    Assert.ArgumentNotNull(context, "context");
            Context.ClientPage.ClientResponse.Broadcast(Context.ClientPage.ClientResponse.ShowModalDialog(
                "/sitecore/shell/default.aspx?xmlcontrol=WordpressImport"),"Shell");
	    }

    }
}