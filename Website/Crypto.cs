using System.Security.Cryptography;
using System.Text;

namespace Sitecore.Modules.WeBlog
{
    public class Crypto
    {
        /// <summary>
        /// Generate an MD5 hash of a string
        /// </summary>
        /// <param name="input">The string to hash</param>
        /// <returns>The MD5 hash</returns>
        public static string GetMD5Hash(string input)
        {
            var prep = input.Trim();
            prep = prep.ToLower();

            var hasher = new MD5CryptoServiceProvider();

            var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(prep));

            var output = new StringBuilder();
            foreach (var hashByte in hash)
                output.Append(hashByte.ToString("X2"));

            return output.ToString().ToLower();
        }
    }
}