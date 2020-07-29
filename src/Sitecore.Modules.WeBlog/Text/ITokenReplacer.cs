using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Text
{
    /// <summary>
    /// Defines a general token replacer.
    /// </summary>
    public interface ITokenReplacer
    {
        /// <summary>
        /// Determines whether the provided input contains a WeBlog settings token.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <returns>True if the text contains the WeBlog settings token.</returns>
        bool ContainsToken(string text);

        /// <summary>
        /// Replace any WeBlog settings tokens in the provided input.
        /// </summary>
        /// <param name="text">The text to replace the tokens in.</param>
        /// <param name="contextItem">The context item used to resolve the settings.</param>
        /// <returns>The replaced text.</returns>
        string Replace(string text, Item contextItem);
    }
}
