using Sitecore.Data.Items;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class ScribanMailActionItem : CustomItem
    {
        public static implicit operator ScribanMailActionItem(Item innerItem)
        {
            return innerItem != null ? new ScribanMailActionItem(innerItem) : null;
        }

        public static implicit operator Item(ScribanMailActionItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public ScribanMailActionItem(Item innerItem) : base(innerItem)
        {
        }

        public string To
        {
            get
            {
                return InnerItem["to"];
            }
        }

        public string From
        {
            get
            {
                return InnerItem["from"];
            }
        }

        public string Subject
        {
            get
            {
                return InnerItem["subject"];
            }
        }

        public string Message
        {
            get
            {
                return InnerItem["message"];
            }
        }
    }
}