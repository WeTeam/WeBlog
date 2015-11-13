﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Items.WeBlog
{
    public partial class BlogHomeItem : CustomItem
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

        /// <summary>
        /// Gets the URL of the blog item
        /// </summary>
        public string Url
        {
            get
            {
                return LinkManager.GetItemUrl(InnerItem);
            }
        }

        /// <summary>
        /// Gets the absolute URL of the blog item including the server
        /// </summary>
        public string AbsoluteUrl
        {
            get
            {
                UrlOptions urlOptions = UrlOptions.DefaultOptions;
                urlOptions.AlwaysIncludeServerUrl = true;
                return LinkManager.GetItemUrl(InnerItem, urlOptions);
            }
        }

        public IEnumerable<Feeds.RSSFeedItem> SyndicationFeeds
        {
            get
            {
                List<Feeds.RSSFeedItem> feeds = null;
                if (this.EnableRSS.Checked)
                {
                    var rssTemplateID = Settings.RssFeedTemplateIDString;
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

        public Item DictionaryItem
        {
            get
            {
                if (CustomDictionaryFolder.Item != null)
                {
                    return CustomDictionaryFolder.Item;
                }
                return ItemManager.GetItem(Settings.DictionaryPath, Context.Language, Sitecore.Data.Version.Latest, Context.Database);
            }
        }

        public BlogSettings BlogSettings
        {
            get
            {
                if (!InnerItem.TemplateIsOrBasedOn(Settings.BlogTemplateID))
                {
                    return new BlogSettings();
                }

                return new BlogSettings
                {
                    CategoryTemplateID = DefinedCategoryTemplate.Field.TargetID,
                    EntryTemplateID = DefinedEntryTemplate.Field.TargetID,
                    CommentTemplateID = DefinedCommentTemplate.Field.TargetID
                };
            }
        }
    }
}