
namespace Sitecore.Modules.WeBlog.Extensions
{
    public static class StringExtensions
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
    }
}
