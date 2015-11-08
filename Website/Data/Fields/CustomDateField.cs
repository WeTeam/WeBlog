using System;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Data.Fields
{
    public class CustomDateField : CustomFieldBase<DateField>
    {
        public DateTime DateTime
        {
            get
            {
                if (InnerField == null || InternalItem.Fields[InnerField.InnerField.Name] == null)
                    return DateTime.MinValue;

                return ((DateField)InternalItem.Fields[InnerField.InnerField.Name]).DateTime;
            }
        }

        public CustomDateField(Item item, DateField field)
            : base(item, field)
        {
        }
    }
}