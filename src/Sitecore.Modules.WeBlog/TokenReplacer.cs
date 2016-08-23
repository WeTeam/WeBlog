using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;
using Sitecore.Modules.WeBlog.Managers;

namespace Sitecore.Modules.WeBlog
{
    public class TokenReplacer
    {
        public static string Replace(string input, Item contextItem)
        {
            input = ResolveContextTokens(input, contextItem);
            input = ResolveSettingsTokens(input);
            return input;
        }

        public static string ResolveSettingsTokens(string text)
        {
            if (text.Contains(Constants.Tokens.WeBlogSetting))
            {
                foreach (var settingName in GetTokensArgs(text, Constants.Tokens.WeBlogSetting))
                {
                    string settingsValue = Sitecore.Configuration.Settings.GetSetting(settingName);

                    if (!string.IsNullOrEmpty(settingsValue))
                    {
                        text = text.Replace(string.Format("{0}({1})", Constants.Tokens.WeBlogSetting, settingName), settingsValue);
                    }
                }
            }
            return text;
        }

        public static string ResolveContextTokens(string text, Item contextItem)
        {
            if (text.Contains(Constants.Tokens.WeBlogContext))
            {
                foreach (var arg in GetTokensArgs(text, Constants.Tokens.WeBlogContext))
                {
                    var token = String.Format("{0}({1})", Constants.Tokens.WeBlogContext, arg);
                    var itemDelegate = GetItemDelegate(arg, contextItem);
                    text = ResolvePath(text, token, itemDelegate);
                }
            }
            return text;
        }

        private static IEnumerable<string> GetTokensArgs(string text, string token)
        {
            var pattern = String.Format(@"(?<=\{0})\(([^)]*)\)", token);
            var matches = Regex.Matches(text, pattern);
            return matches.Cast<Match>().Select(m => m.Value.Trim('(', ')')).Distinct();
        }

        private static Func<Item> GetItemDelegate(string arg, Item contextItem)
        {
            switch (arg.ToLower())
            {
                case "categoryroot":
                    {
                        return () => ManagerFactory.CategoryManagerInstance.GetCategoryRoot(contextItem);
                    }
            }
            return () => contextItem;
        }

        private static string ResolvePath(string text, string token, Func<Item> itemDelegate)
        {
            if (text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                Item item = itemDelegate();
                if (item != null)
                {
                    text = text.Replace(token, item.Paths.Path);
                }
            }
            return text.Replace(token, string.Empty);
        }
    }
}
