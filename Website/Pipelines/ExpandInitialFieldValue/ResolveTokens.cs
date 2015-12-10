using Sitecore.Pipelines.ExpandInitialFieldValue;

namespace Sitecore.Modules.WeBlog.Pipelines.ExpandInitialFieldValue
{
    public class ResolveTokens : ExpandInitialFieldValueProcessor
    {
        public override void Process(ExpandInitialFieldValueArgs args)
        {
            if (args.SourceField.Value.Contains(Constants.Tokens.WeBlogSetting))
            {
                args.Result = TokenReplacer.ResolveSettingsTokens(args.Result);
            }
        }
    }
}