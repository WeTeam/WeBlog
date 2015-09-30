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

        /// <summary>
        /// Limits a strings length to a maximum number of characters
        /// </summary>
        /// <param name="input">The input to process</param>
        /// <param name="length">The maximum length of the string</param>
        /// <param name="continuesToken">The token to append to the string if it is truncated</param>
        /// <returns>A string with less than or the same number of characters as specified in the length</returns>
        public static string MaxLength(this string input, int length, string continuesToken = "...")
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if (input.Length <= length)
                return input;

            var originalCount = length - continuesToken.Length - 1;
            if(originalCount < 0)
                originalCount = 0;

            return input.Substring(0, originalCount) + continuesToken;
        }
    }
}
