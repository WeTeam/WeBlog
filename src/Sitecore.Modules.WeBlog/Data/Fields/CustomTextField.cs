using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Data.Fields
{
    public class CustomTextField : CustomFieldBase<TextField>
    {
        public string Text
        {
            get { return Rendered; }
        }

        public CustomTextField(Item item, TextField field)
            : base(item, field)
        {

        }
    }
}