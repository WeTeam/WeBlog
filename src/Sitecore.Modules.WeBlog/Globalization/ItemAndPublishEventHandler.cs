using System;
using Sitecore.Abstractions;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Modules.WeBlog.Caching;
using Sitecore.Modules.WeBlog.Configuration;
using Sitecore.Modules.WeBlog.Diagnostics;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Globalization
{
    public class ItemAndPublishEventHandler
    {
        private IWeBlogSettings _settings = null;

        private BaseTemplateManager _templateManager = null;

        public ItemAndPublishEventHandler()
            : this(WeBlogSettings.Instance)
        {
        }

        [Obsolete("Use ctor(IWeBlogSettings, BaseTemplateManager) instead.")]
        public ItemAndPublishEventHandler(IWeBlogSettings settings)
            : this(settings, ServiceLocator.ServiceProvider.GetService(typeof(BaseTemplateManager)) as BaseTemplateManager)
        {
        }

        public ItemAndPublishEventHandler(IWeBlogSettings settings, BaseTemplateManager templateManager)
        {
            Assert.ArgumentNotNull(settings, nameof(settings));
            Assert.ArgumentNotNull(templateManager, nameof(templateManager));

            _settings = settings;
            _templateManager = templateManager;
        }



        public void OnItemSaved(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            Item item = Event.ExtractParameter(args, 0) as Item;
            HandleOnItemSaved(item);
        }

        public void OnItemSavedRemote(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            var args2 = args as ItemSavedRemoteEventArgs;
            if (args2 == null)
            {
                return;
            }
            HandleOnItemSaved(args2.Item);
        }

        protected void HandleOnItemSaved(Item item)
        {
            if (item != null)
            {
                if (item.TemplateIsOrBasedOn(_templateManager, _settings.DictionaryEntryTemplateId))
                {
                    Logger.Info("Dictionary Entry saved, clearing Translator cache", this);
                    CacheManager.TranslatorCache.ClearCache();
                }
                if (item.TemplateIsOrBasedOn(_templateManager, _settings.ProfanityListTemplateId))
                {
                    Logger.Info("Profanity Filter item saved, clearing cache", this);
                    CacheManager.ProfanityFilterCache.ClearCache();
                }
            }
        }

        public void OnPublishEnd(object sender, EventArgs args)
        {
            Logger.Info("Publish complete, clearing Translator cache", this);
            CacheManager.TranslatorCache.ClearCache();
        }
    }
}
