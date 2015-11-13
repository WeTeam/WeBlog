// Need to use an alias for AntiXSS lib as it redefines certain types from CLR and doesn't compile when target framework is 3.5 (for Sitecore 6.2 build)
extern alias antixss;

using CustomItemGenerator.Fields.SimpleTypes;
using antixss::Microsoft.Security.Application;

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
            //return AntiXss.HtmlEncode(field.Raw);
            return Encoder.HtmlEncode(field.Raw);
        }
    }
}