using Sitecore.Data.Items;
using Sitecore.Mvc.Helpers;
using System;
using System.Web;

namespace Sitecore.Modules.WeBlog.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString Field(this SitecoreHelper htmlHelper, string fieldName, bool shouldWrap)
        {
            return Field(htmlHelper, fieldName, null, null, shouldWrap);
        }

        public static HtmlString Field(this SitecoreHelper htmlHelper, string fieldName, object parameters, bool shouldWrap)
        {
            return Field(htmlHelper, fieldName, null, parameters, shouldWrap);
        }

        public static HtmlString Field(this SitecoreHelper htmlHelper, string fieldName, Item item, bool shouldWrap)
        {
            return Field(htmlHelper, fieldName, item, null, shouldWrap);
        }

        public static HtmlString Field(this SitecoreHelper htmlHelper, string fieldName, Item item, object parameters, bool shouldWrap)
        {
            string format = shouldWrap ? "<p>{0}{1}</p>" : "{0}{1}";
            var value = String.Format(format,
                htmlHelper.BeginField(fieldName, item, parameters),
                htmlHelper.EndField());
            return new HtmlString(value);
        }
    }
}