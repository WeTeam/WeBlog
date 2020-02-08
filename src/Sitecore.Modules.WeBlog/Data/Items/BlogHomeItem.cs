using System;
using System.Collections.Generic;
using System.Drawing;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Extensions;
using Sitecore.Modules.WeBlog.Data.Fields;
using Sitecore.Modules.WeBlog.Configuration;

#if FEATURE_ABSTRACTIONS
using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Sites;
#endif

#if SC93
using Sitecore.Links.UrlBuilders;
#endif

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class BlogHomeItem : CustomItem
    {
        protected IWeBlogSettings Settings { get; }

#if FEATURE_ABSTRACTIONS
        private BaseLinkManager _linkManager = null;
#endif

        public BlogHomeItem(Item innerItem, IWeBlogSettings settings = null)
#if FEATURE_ABSTRACTIONS
            : this(innerItem, ServiceLocator.ServiceProvider.GetService(typeof(BaseLinkManager)) as BaseLinkManager, settings)
        {
        }
#else
            : base(innerItem)
        {
            Settings = settings ?? WeBlogSettings.Instance;
        }
#endif

#if FEATURE_ABSTRACTIONS
        public BlogHomeItem(Item innerItem, BaseLinkManager linkManager, IWeBlogSettings settings = null)
            : base(innerItem)
        {
            if (linkManager == null)
                throw new ArgumentNullException(nameof(linkManager));

            _linkManager = linkManager;
            Settings = settings ?? WeBlogSettings.Instance;
        }

        public static implicit operator BlogHomeItem(Item innerItem)
        {
            var linkManager = ServiceLocator.ServiceProvider.GetService(typeof(BaseLinkManager)) as BaseLinkManager;
            return innerItem != null ? new BlogHomeItem(innerItem, linkManager) : null;
        }
#else

        public static implicit operator BlogHomeItem(Item innerItem)
        {
            return innerItem != null ? new BlogHomeItem(innerItem) : null;
        }
#endif

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


        public CustomTextField DisplayItemCount
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["DisplayItemCount"]); }
        }


        public CustomCheckboxField EnableLiveWriter
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["EnableLiveWriter"]); }
        }


        public CustomTextField DisplayCommentSidebarCount
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["DisplayCommentSidebarCount"]); }
        }


        public CustomCheckboxField EnableGravatar
        {
            get { return new CustomCheckboxField(InnerItem, InnerItem.Fields["Enable Gravatar"]); }
        }


        public CustomTextField GravatarSize
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["Gravatar Size"]); }
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
#if SC93
                var urlOptions = new ItemUrlBuilderOptions();
#else
                var urlOptions = UrlOptions.DefaultOptions;
#endif

                urlOptions.AlwaysIncludeServerUrl = true;

#if FEATURE_ABSTRACTIONS
                return _linkManager.GetItemUrl(InnerItem, urlOptions);
#else
                return LinkManager.GetItemUrl(InnerItem, urlOptions);
#endif
            }
        }

        public IEnumerable<RssFeedItem> SyndicationFeeds
        {
            get
            {
                if (EnableRss.Checked)
                {
                    var children = InnerItem.GetChildren();
                    foreach(Item child in children)
                    {
                        if (child.TemplateIsOrBasedOn(Settings.RssFeedTemplateIds))
                            yield return new RssFeedItem(child);
                    }
                }
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
                if (!InnerItem.TemplateIsOrBasedOn(Settings.BlogTemplateIds))
                {
                    return new BlogSettings(Settings);
                }

                return new BlogSettings(Settings)
                {
                    CategoryTemplateID = DefinedCategoryTemplate.Field.TargetID,
                    EntryTemplateID = DefinedEntryTemplate.Field.TargetID,
                    CommentTemplateID = DefinedCommentTemplate.Field.TargetID
                };
            }
        }
    }
}