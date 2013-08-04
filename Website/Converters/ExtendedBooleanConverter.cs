using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Sitecore.Modules.WeBlog.Converters
{
    public class ExtendedBooleanConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                var strValue = (value as string).ToLower();

                switch (strValue)
                {
                    case "1":
                    case "true":
                    case "yes":
                    case "y":
                        return true;

                    case "":
                    case "0":
                    case "false":
                    case "no":
                    case "n":
                        return false;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}