using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.Custom
{
    /// <summary>
    /// Custom field for Checkbox fields
    /// </summary>
    public class CustomCheckboxField : CustomFieldBase<CheckboxField>
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Checked
        {
            get
            {
                if (InnerField == null || InternalItem.Fields[InnerField.InnerField.Name] == null)
                    return false;
                
                return ((CheckboxField) InternalItem.Fields[InnerField.InnerField.Name]).Checked;
            }
        }

        public CustomCheckboxField(Item item, CheckboxField field)
            : base(item, field)
        {
        }
    }
}