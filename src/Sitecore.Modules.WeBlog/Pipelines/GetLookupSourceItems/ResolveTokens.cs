using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Text;
using Sitecore.Pipelines.GetLookupSourceItems;

namespace Sitecore.Modules.WeBlog.Pipelines.GetLookupSourceItems
{
    public class ResolveTokens
    {
        /// <summary>
        /// Gets the <see cref="IContextTokenReplacer"/> used to replace the WeBlog tokens.
        /// </summary>
        protected IContextTokenReplacer TokenReplacer { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="tokenReplacer">The <see cref="IContextTokenReplacer"/> used to replace the WeBlog tokens.</param>
        public ResolveTokens(IContextTokenReplacer tokenReplacer)
        {
            TokenReplacer = tokenReplacer ?? ServiceLocator.ServiceProvider.GetRequiredService<IContextTokenReplacer>();
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public ResolveTokens()
            : this(null)
        {
        }

        public void Process(GetLookupSourceItemsArgs args)
        {
            if (TokenReplacer.ContainsToken(args.Source))
            {
                args.Source = TokenReplacer.Replace(args.Source, args.Item);
            }
        }
    }
}