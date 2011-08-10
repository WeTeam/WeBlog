using Sitecore.Search;
using Sitecore.Shell.Applications.Dialogs.ProgressBoxes;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.StringExtensions;
using System;

namespace Sitecore.Modules.WeBlog.Commands
{
    public class RebuildIndex : Command/*, ISupportsContinuation*/
    {
        public override void Execute(CommandContext context)
        {
            //ContinuationManager.Current.Start(this, "Run", new ClientPipelineArgs());
            Context.ClientPage.Start(this, "Confirm", new ClientPipelineArgs());
        }

        protected void Confirm(ClientPipelineArgs args)
        {
            if (!args.IsPostBack)
            {
                SheerResponse.Confirm("Are you sure you want to rebuild the WeBlog search index?");
                args.WaitForPostBack();
            }
            else
            {
                if (args.Result == "yes")
                {
                    ProgressBox.Execute("weblog-index-rebuild", "Rebuilding WeBlog Search Index", new ProgressBoxMethod(Run));
                }
            }
        }

        protected void Run(params object[] parameters)
        {
            var indexName = Settings.SearchIndexName;

            if (!string.IsNullOrEmpty(indexName))
            {
                Log("Locating WeBlog index '{0}'".FormatWith(indexName));
                var index = SearchManager.GetIndex(indexName);

                if (index != null)
                {
                    Log("Index found. Starting index rebuild");
                    IncrementProgress();
                    index.Rebuild();
                }
            }
        }

        protected void Log(string message)
        {
            if (Context.Job != null)
                Context.Job.Status.Messages.Add(message);
        }

        protected void IncrementProgress()
        {
            if (Context.Job != null)
                Context.Job.Status.Processed++;
        }
    }
}