using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Managers;
using System.Text.RegularExpressions;

namespace Sitecore.Modules.WeBlog.Text
{
    public class ContextTokenReplacer : IContextTokenReplacer
    {
        /// <summary>
        /// The WeBlog context token.
        /// </summary>
        public const string Token = "$weblogcontext";

        /// <summary>
        /// The <see cref="Regex"/> used to parse and replace the tokens from the input.
        /// </summary>
        protected static Regex _tokenRegex = new Regex($@"\{Token}\(([^)]*)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Gets the <see cref="ICategoryManager"/> instance used to resolve the categories root.
        /// </summary>
        protected ICategoryManager CategoryManager { get; }

        /// <summary>
        /// The context item to operate with.
        /// </summary>
        protected Item ContextItem { get; set; }

        public ContextTokenReplacer(ICategoryManager categoryManager)
        {
            Assert.ArgumentNotNull(categoryManager, nameof(categoryManager));

            CategoryManager = categoryManager;
        }

        public bool ContainsToken(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return text.Contains(Token);
        }

        public string Replace(string text, Item contextItem)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            ContextItem = contextItem;

            var result = _tokenRegex.Replace(text, RegexMatchProcessor);

            return result;
        }

        private string RegexMatchProcessor(Match match)
        {
            // Using the match groups, group 0 is the entire match and group 1 is the argument.

            var item = ContextItem;
            if (match.Groups.Count == 2)
            {
                var argument = match.Groups[1].Value;

                switch (argument.ToLower())
                {
                    case "categoryroot":
                        item = CategoryManager.GetCategoryRoot(item);
                        break;
                    default:
                        return match.Value;
                }
            }

            return item.Paths.Path;
        }
    }
}
