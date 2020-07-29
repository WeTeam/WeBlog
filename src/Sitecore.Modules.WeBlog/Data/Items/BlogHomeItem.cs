using System;
using System.Collections.Generic;
using System.Drawing;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Links;
using Sitecore.Modules.WeBlog.Data.Fields;
using Sitecore.Modules.WeBlog.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Modules.WeBlog.Managers;

#if SC93
using Sitecore.Links.UrlBuilders;
#endif

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class BlogHomeItem : CustomItem
    {
        [Obsolete("No longer used. If required, resolve this member from the service provider.")]
        protected IWeBlogSettings Settings { get; }

        public BlogHomeItem(Item innerItem)
            : base(innerItem)
        {
        }

        [Obsolete("Use ctor(Item) instead.")]
        public BlogHomeItem(Item innerItem, IWeBlogSettings settings = null)
            : base(innerItem)
        {
            Settings = settings;
        }

        [Obsolete("Use ctor() instead.")]
        public BlogHomeItem(Item innerItem, BaseLinkManager linkManager, IWeBlogSettings settings = null)
            : base(innerItem)
        {
            Settings = settings;
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
        [Obsolete("Use the Sitecore LinkManager instead.")]
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
        [Obsolete("Use BaseLinkManager.GetItemUrl() with this item instead.")]
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
                return LinkManager.GetItemUrl(InnerItem, urlOptions);
            }
        }
        
        [Obsolete("Use Sitecore.Modules.WeBlog.Data.IFeedResolver instead.")]
        public IEnumerable<RssFeedItem> SyndicationFeeds
        {
            get
            {
                var resolver = ServiceLocator.ServiceProvider.GetRequiredService<IFeedResolver>();
                return resolver.Resolve(this);
            }
        }

        [Obsolete("Use Sitecore.Modules.WeBlog.Managers.BlogManager.GetDictionaryItem() instead.")]
        public Item DictionaryItem
        {
            get
            {
                var manager = ServiceLocator.ServiceProvider.GetRequiredService<IBlogManager>();
                return manager.GetDictionaryItem();
            }
        }

        [Obsolete("Use Sitecore.Modules.WeBlog.Configuration.IBlogSettingsResolver instead.")]
        public BlogSettings BlogSettings
        {
            get
            {
                var resolver = ServiceLocator.ServiceProvider.GetRequiredService<IBlogSettingsResolver>();
                return resolver.Resolve(this);
            }
        }
    }
}