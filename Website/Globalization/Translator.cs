using Sitecore.Modules.WeBlog.Caching;
using Sitecore.Web.UI.WebControls;

namespace Sitecore.Modules.WeBlog.Globalization
{
    public class Translator
    {
        private const string PHRASE = "Phrase";

        /// <summary>
        /// Renders the specified key as text
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Text(string key)
        {
            var entry = CacheManager.TranslatorCache.FindEntry(key);
            if (entry == null)
            {
                return "#" + key + "#";
            }
            return entry[PHRASE];
        }

        /// <summary>
        /// Renders the specified key with a fieldrenderer.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string Render(string key)
        {
            var entry = CacheManager.TranslatorCache.FindEntry(key);
            if (entry == null)
            {
                return "#" + key + "#";
            }
            return FieldRenderer.Render(entry, PHRASE);
        }

        public static string Render(string key, bool disableWebEditing)
        {
            var entry = CacheManager.TranslatorCache.FindEntry(key);
            if (entry == null)
            {
                return "#" + key + "#";
            }

            if (disableWebEditing)
                return FieldRenderer.Render(entry, PHRASE, "disable-web-editing=true");
            else
                return FieldRenderer.Render(entry, PHRASE);
        }

        /// <summary>
        /// Renders the specified key as a formatted string
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="arg0">The arg0.</param>
        /// <returns></returns>
        public static string Format(string key, object arg0)
        {
            string text = Text(key);
            return string.Format(text, arg0);
        }

        /// <summary>
        /// Renders the specified key as a formatted string
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static string Format(string key, params object[] args)
        {
            string text = Text(key);
            return string.Format(text, args);
        }
    }
}