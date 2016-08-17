using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace Sitecore.Modules.WeBlog.Data.Fields
{
    public class CustomGeneralLinkField : CustomFieldBase<LinkField>
    {
        public string Url
        {
            get
            {
                if (InnerField == null || InternalItem.Fields[this.InnerField.InnerField.Name] == null)
                    return null;

                if (InnerField.IsInternal)
                {
                    var obj = Context.Database.GetItem(InnerField.TargetID);
                    return obj == null ? string.Empty : LinkManager.GetItemUrl(obj);
                }
                
                if (InnerField.IsMediaLink)
                {
                    return InnerField.TargetItem == null ? string.Empty : StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl((MediaItem)InnerField.TargetItem));
                }
                
                return InnerField.Url ?? string.Empty;
            }
        }

        public CustomGeneralLinkField(Item item, LinkField field)
            : base(item, field)
        {
        }
    }
}