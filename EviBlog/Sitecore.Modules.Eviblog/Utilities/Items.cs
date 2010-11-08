using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sitecore.Modules.Eviblog.Utilities
{
    /// <summary>
    /// Provides utilities for working with Sitecore items
    /// </summary>
    public static class Items
    {
        private static readonly string SAFE_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789- ";

        /// <summary>
        /// Generates a safe item name from the given input which can be used to create an item in the content tree
        /// </summary>
        /// <param name="input">The desired item name</param>
        /// <returns>A safe item name</returns>
        public static string MakeSafeItemName(string input)
        {
            var output = new StringBuilder();
            foreach (var c in input)
            {
                if (SAFE_CHARS.Contains(c))
                    output.Append(c);
                else
                    output.Append('-');
            }

            return output.ToString();
        }
    }
}
