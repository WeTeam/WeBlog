using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Text;
using Sitecore.Pipelines.ExpandInitialFieldValue;

namespace Sitecore.Modules.WeBlog.Pipelines.ExpandInitialFieldValue
{
    public class ResolveTokens : ExpandInitialFieldValueProcessor
    {
        /// <summary>
        /// Gets the <see cref="ITokenReplacer"/> used to replace the WeBlog tokens.
        /// </summary>
        protected ISettingsTokenReplacer TokenReplacer { get; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="tokenReplacer">The <see cref="ITokenReplacer"/> used to replace the WeBlog tokens.</param>
        public ResolveTokens(ISettingsTokenReplacer tokenReplacer)
        {
            TokenReplacer = tokenReplacer ?? ServiceLocator.ServiceProvider.GetRequiredService<ISettingsTokenReplacer>();
        }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        public ResolveTokens()
            : this(null)
        {
        }

        public override void Process(ExpandInitialFieldValueArgs args)
        {
            if (TokenReplacer.ContainsToken(args.Result))
            {
                args.Result = TokenReplacer.Replace(args.Result, args.TargetItem);
            }
        }
    }
}