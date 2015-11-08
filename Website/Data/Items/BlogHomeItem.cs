using System;
using System.Collections.Generic;
using System.Drawing;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Data.Fields;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class BlogHomeItem : CustomItem
    {

        public static readonly string TemplateId = "{46663E05-A6B8-422A-8E13-36CD2B041278}";


        public BlogHomeItem(Item innerItem)
            : base(innerItem)
        {

        }

        public static implicit operator BlogHomeItem(Item innerItem)
        {
            return innerItem != null ? new BlogHomeItem(innerItem) : null;
        }

        public static implicit operator Item(BlogHomeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        public CustomLookupField DefinedCategoryTemplate
        {
            get { return new CustomLookupField(InnerItem, InnerItem.Fields["Defined Category Template"]); }
        }


        public CustomCheckboxField EnableRss
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable RSS"]); }
        }


        public CustomTextField Title
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Title"]); }
        }


        public CustomTextField TitleCategories
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["titleCategories"]); }
        }


        public CustomLookupField DefinedEntryTemplate
        {
            get { return new CustomLookupField(InnerItem, InnerItem.Fields["Defined Entry Template"]); }
        }


        public CustomTextField Email
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Email"]); }
        }


        public CustomCheckboxField EnableComments
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable Comments"]); }
        }


        public CustomTextField TitleRecentComments
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["titleRecentComments"]); }
        }


        public CustomLookupField DefinedCommentTemplate
        {
            get { return new CustomLookupField(InnerItem, InnerItem.Fields["Defined Comment Template"]); }
        }


        public CustomCheckboxField ShowEmailWithinComments
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["Show Email Within Comments"]); }
        }


        public CustomLookupField Theme
        {
            get { return new CustomLookupField(InnerItem, InnerItem.Fields["Theme"]); }
        }


        public CustomTextField TitleAdministration
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["titleAdministration"]); }
        }


        public CustomTextField DisplayItemCount
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["DisplayItemCount"]); }
        }


        public CustomCheckboxField EnableLiveWriter
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["EnableLiveWriter"]); }
        }


        public CustomTextField TitleTagcloud
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["titleTagcloud"]); }
        }


        public CustomTextField DisplayCommentSidebarCount
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["DisplayCommentSidebarCount"]); }
        }


        public CustomCheckboxField EnableGravatar
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable Gravatar"]); }
        }


        public CustomTextField TitleComments
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["titleComments"]); }
        }


        public CustomTextField GravatarSize
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Gravatar Size"]); }
        }


        public CustomTextField MaximumGeneratedIntroductionCharacters
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Maximum Generated Introduction Characters"]); }
        }


        public CustomTextField TitleAddYourComment
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["titleAddYourComment"]); }
        }


        public CustomMultiListField DefaultGravatarStyle
        {
            get { return new CustomMultiListField(InnerItem, InnerItem.Fields["Default Gravatar Style"]); }
        }


        public CustomTextField MaximumEntryImageSize
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Maximum Entry Image Size"]); }
        }


        public CustomMultiListField GravatarRating
        {
            get { return new CustomMultiListField(InnerItem, InnerItem.Fields["Gravatar Rating"]); }
        }


        public CustomTextField MaximumThumbnailImageSize
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Maximum Thumbnail Image Size"]); }
        }


        public CustomLookupField CustomDictionaryFolder
        {
            get { return new CustomLookupField(InnerItem, InnerItem.Fields["Custom Dictionary Folder"]); }
        }

        public int DisplayItemCountNumeric
        {
            get
            {
                var count = 0;
                Int32.TryParse(DisplayItemCount.Raw, out count);
                return count;
            }
        }

        public int DisplayCommentSidebarCountNumeric
        {
            get
            {
                var count = 0;
                Int32.TryParse(DisplayCommentSidebarCount.Raw, out count);
                return count;
            }
        }

        public int MaximumGeneratedIntroductionCharactersNumeric
        {
            get
            {
                var count = 0;
                Int32.TryParse(MaximumGeneratedIntroductionCharacters.Raw, out count);
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
            if (string.IsNullOrEmpty(fieldValue)) return Size.Empty;

            var numbers = fieldValue.Split(new char[] { ',', 'x', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (numbers.Length < 2) return Size.Empty;

            var width = 0;
            int.TryParse(numbers[0], out width);

            var height = 0;
            int.TryParse(numbers[1], out height);

            return new Size(width, height);
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
                var urlOptions = UrlOptions.DefaultOptions;
                urlOptions.AlwaysIncludeServerUrl = true;
                return LinkManager.GetItemUrl(InnerItem, urlOptions);
            }
        }

        public IEnumerable<RssFeedItem> SyndicationFeeds
        {
            get
            {
                List<RssFeedItem> feeds = null;
                if (this.EnableRss.Checked)
                {
                    var rssTemplateId = Settings.RssFeedTemplateIDString;
                    var feedItems = this.InnerItem.Axes.SelectItems(string.Format("./*[@@templateid='{0}']", rssTemplateId));
                     
                    if (feedItems != null)
                    {
                        feeds = new List<RssFeedItem>();
                        foreach (Item item in feedItems)
                        {
                            feeds.Add(new RssFeedItem(item));
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