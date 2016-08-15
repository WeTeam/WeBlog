using Microsoft.Security.Application;
using Sitecore.Modules.WeBlog.Data.Fields;

namespace Sitecore.Modules.WeBlog.Extensions
{
    public static class CustomTextFieldExtensions
    {
        /// <summary>
        /// Gets the field value in a web safe escaped format
        /// </summary>
        /// <param name="field">The field to get the value for</param>
        /// <returns>The escaped value</returns>
        public static string HtmlEncode(this CustomTextField field)
        {
          return Encoder.HtmlEncode(field.Raw);
        }
    }
}