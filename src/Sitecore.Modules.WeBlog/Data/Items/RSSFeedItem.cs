using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Data.Fields;
using Sitecore.Syndication;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class RssFeedItem : CustomItem
    {
        public RssFeedItem(Item innerItem) : base(innerItem) {}

        public static implicit operator RssFeedItem(Item innerItem)
        {
            return innerItem != null ? new RssFeedItem(innerItem) : null;
        }

        public static implicit operator Item(RssFeedItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public CustomCheckboxField Cacheable
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["Cacheable"]); }
        }


        public CustomImageField Image
        {
            get { return new CustomImageField(InnerItem, InnerItem.Fields["Image"]); }
        }

        public CustomTextField Type
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Type"]); }
        }


        public CustomTextField CacheDuration
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Cache Duration"]); }
        }


        public CustomTextField Copyright
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Copyright"]); }
        }


        public CustomTextField Title
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Title"]); }
        }


        public CustomTextField Description
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Description"]); }
        }


        public CustomTextField Managingeditor
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Managing editor"]); }
        }


        public CustomGeneralLinkField Link
        {
            get { return new CustomGeneralLinkField(InnerItem, InnerItem.Fields["Link"]); }
        }

        public string Url
        {
            get
            {
                var feed = FeedManager.GetFeed(InnerItem);
                return feed.GetUrl(UrlOptions.DefaultOptions, false);
            }
        }
    }
}