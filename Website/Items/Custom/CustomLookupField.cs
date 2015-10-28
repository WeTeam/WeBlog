using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.Custom
{
    public class CustomLookupField : CustomFieldBase<LookupField>
    {
        public Item Item
        {
            get { return InnerField == null ? null : InnerField.TargetItem; }
        }

        public CustomLookupField(Item item, LookupField field)
            : base(item, field)
        {
        }
    }
}