using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Items.Custom
{
    public class CustomMultiListField : CustomFieldBase<MultilistField>
    {
        public IEnumerable<Item> ListItems
        {
            get
            {
                if (InnerField == null || InternalItem.Fields[this.InnerField.InnerField.Name] == null)
                    return new List<Item>();

                return ((MultilistField) this.InternalItem.Fields[this.InnerField.InnerField.Name]).GetItems().ToList();
            }
        }

        public CustomMultiListField(Item item, MultilistField field)
            : base(item, field)
        {
        }
    }
}