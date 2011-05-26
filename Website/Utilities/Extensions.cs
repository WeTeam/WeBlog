using System.Text.RegularExpressions;

namespace Sitecore.Modules.Blog.Utilities
{
    public static class Extensions
    {
        /// <summary>
        /// Determines whether the specified input has value.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// 	<c>true</c> if the specified input has value; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasValue(this string input)
        {
            if (input == null)
            {
                return false;
            }

            if (input.Trim() == string.Empty)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified input is GUID.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// 	<c>true</c> if the specified input is GUID; otherwise, <c>false</c>.
        /// </returns>
        /*public static bool IsGuid(this string input)
        {
            Regex isGuid = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled);
            bool isValid = false;
            if (input != null)
            {
                if (isGuid.IsMatch(input))
                {
                    isValid = true;
                }
            }
            return isValid;
        }*/
    }
}
