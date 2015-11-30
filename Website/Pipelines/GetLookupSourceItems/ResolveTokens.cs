using Sitecore.Pipelines.GetLookupSourceItems;

namespace Sitecore.Modules.WeBlog.Pipelines.GetLookupSourceItems
{
    public class ResolveTokens
    {
        public void Process(GetLookupSourceItemsArgs args)
        {
            if (args.Source.Contains(Constants.Tokens.WeBlogContext))
            {
                args.Source = TokenReplacer.ResolveContextTokens(args.Source, args.Item);
            }
        }
    }
}