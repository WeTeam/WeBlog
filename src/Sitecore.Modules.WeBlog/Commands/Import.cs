using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

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