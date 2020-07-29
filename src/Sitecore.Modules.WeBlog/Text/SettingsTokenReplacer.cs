using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Managers;
using System.Text.RegularExpressions;

namespace Sitecore.Modules.WeBlog.Text
{
    public class SettingsTokenReplacer : ISettingsTokenReplacer
    {
        /// <summary>
        /// The WeBlog settings token.
        /// </summary>
        public const string Token = "$weblogsetting";

        /// <summary>
        /// The <see cref="Regex"/> used to parse and replace the tokens from the input.
        /// </summary>
        protected static Regex _tokenRegex = new Regex($@"\{Token}\(([^)]*)\)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Gets the <see cref="IBlogManager"/> instance used to resolve the current blog.
        /// </summary>
        protected IBlogManager BlogManager { get; }

        /// <summary>
        /// Gets the <see cref="IBlogSettingsResolver"/> used to resolve the settings for the current blog.
        /// </summary>
        protected IBlogSettingsResolver BlogSettingsResolver { get; }

        /// <summary>
        /// The settings to use for the current operation.
        /// </summary>
        protected BlogSettings CurrentSettings { get; set; }

        public SettingsTokenReplacer(IBlogManager blogManager, IBlogSettingsResolver blogSettingsResolver)
        {
            Assert.ArgumentNotNull(blogManager, nameof(blogManager));
            Assert.ArgumentNotNull(blogSettingsResolver, nameof(blogSettingsResolver));

            BlogManager = blogManager;
            BlogSettingsResolver = blogSettingsResolver;
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

            var blogHomeItem = BlogManager.GetCurrentBlog(contextItem);
            var blogSettings = BlogSettingsResolver.Resolve(blogHomeItem);
            CurrentSettings = blogSettings;

            var result = _tokenRegex.Replace(text, RegexMatchProcessor);

            return result;
        }

        private string RegexMatchProcessor(Match match)
        {
            // Using the match groups, group 0 is the entire match and group 1 is the argument.

            if (match.Groups.Count == 2)
            {
                var settingName = match.Groups[1].Value;

                // The old implementation used reflection, which seems like overkill for these 3 simple properties.
                switch (settingName.ToLower())
                {
                    case "categorytemplateid":
                        return CurrentSettings.CategoryTemplateID.ToString();

                    case "commenttemplateid":
                        return CurrentSettings.CommentTemplateID.ToString();

                    case "entrytemplateid":
                        return CurrentSettings.EntryTemplateID.ToString();
                }
            }

            return match.Value;
        }
    }
}
