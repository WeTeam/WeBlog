using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace Sitecore.Modules.WeBlog.Items.Custom
{
    public class CustomImageField : CustomFieldBase<ImageField>
    {
        public MediaItem MediaItem
        {
            get
            {
                if (InnerField == null || InternalItem.Fields[InnerField.InnerField.Name] == null)
                    return null;
                return ((ImageField)InternalItem.Fields[InnerField.InnerField.Name]).MediaItem;
            }
        }

        public string MediaUrl
        {
            get { return MediaItem == null ? string.Empty : StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(MediaItem)); }
        }

        public CustomImageField(Item item, ImageField field)
            : base(item, field)
        {
        }
    }
}