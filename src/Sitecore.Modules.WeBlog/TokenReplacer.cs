using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Modules.WeBlog.Text;
using System;

namespace Sitecore.Modules.WeBlog
{
    [Obsolete("Use either Sitecore.Modules.WeBlog.Text.IContextTokenReplacer or Sitecore.Modules.WeBlog.Text.ISettingsTokenReplacer.")]
    public class TokenReplacer
    {
        public static string Replace(string input, Item contextItem)
        {
            input = ResolveContextTokens(input, contextItem);
            input = ResolveSettingsTokens(input, contextItem);
            return input;
        }

        public static string ResolveSettingsTokens(string text, Item contextItem)
        {
            var replacer = ServiceLocator.ServiceProvider.GetRequiredService<ISettingsTokenReplacer>();
            if (replacer.ContainsToken(text))
                return replacer.Replace(text, contextItem);

            return text;
        }

        public static string ResolveContextTokens(string text, Item contextItem)
        {
            var replacer = ServiceLocator.ServiceProvider.GetRequiredService<IContextTokenReplacer>();
            if (replacer.ContainsToken(text))
                return replacer.Replace(text, contextItem);

            return text;
        }
    }
}
