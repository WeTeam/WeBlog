using System;
using Sitecore.Data.Items;
using System.Collections.Generic;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using CustomItemGenerator.Fields.LinkTypes;
using CustomItemGenerator.Fields.ListTypes;
using CustomItemGenerator.Fields.SimpleTypes;
using Sitecore.Web;
using Sitecore.Links;
using System.Drawing;

namespace Sitecore.Modules.Blog.Items.Blog
{
    public partial class BlogItem : CustomItem
    {

        public int DisplayItemCountNumeric
        {
            get
            {
                var count = 0;
                Int32.TryParse(this.DisplayItemCount.Raw, out count);
                return count;
            }
        }

        public int DisplayCommentSidebarCountNumeric
        {
            get
            {
                var count = 0;
                Int32.TryParse(this.DisplayCommentSidebarCount.Raw, out count);
                return count;
            }
        }

        public int MaximumGeneratedIntroductionCharactersNumeric
        {
            get
            {
                var count = 0;
                Int32.TryParse(this.MaximumGeneratedIntroductionCharacters.Raw, out count);
                return count;
            }
        }

        public int GravatarSizeNumeric
        {
            get
            {
                var size = 50;
                int.TryParse(GravatarSize.Raw, out size);
                return size;
            }
        }

        public Size MaximumEntryImageSizeDimension
        {
            get
            {
                var size = ParseDimension(MaximumEntryImageSize.Raw);
                return size != Size.Empty ? size : new Size(300, 300);
            }
        }

        public Size MaximumThumbnailImageSizeDimension
        {
            get
            {
                var size = ParseDimension(MaximumThumbnailImageSize.Raw);
                return size != Size.Empty ? size : new Size(50, 50);
            }
        }

        protected Size ParseDimension(string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                var numbers = fieldValue.Split(new char[] { ',', 'x', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (numbers.Length >= 2)
                {
                    var width = 0;
                    int.TryParse(numbers[0], out width);

                    var height = 0;
                    int.TryParse(numbers[1], out height);

                    return new Size(width, height);
                }
            }

            return Size.Empty;
        }

        public string Url
        {
            get
            {
                return "http://" + WebUtil.GetHostName() + LinkManager.GetItemUrl(InnerItem);
            }
        }

        public IEnumerable<Feeds.RSSFeedItem> SyndicationFeeds
        {
            get
            {
                List<Feeds.RSSFeedItem> feeds = null;
                if (this.EnableRSS.Checked)
                {
                    var rssTemplateID = Sitecore.Configuration.Settings.GetSetting("Blog.RSSFeedTemplateID");
                    Item[] feedItems = this.InnerItem.Axes.SelectItems(string.Format("./*[@@templateid='{0}']", rssTemplateID));
                    if (feedItems != null)
                    {
                        feeds = new List<Feeds.RSSFeedItem>();
                        foreach (Item item in feedItems)
                        {
                            feeds.Add(new Feeds.RSSFeedItem(item));
                        }
                    }
                }
                return feeds;
            }
        }

    }
}