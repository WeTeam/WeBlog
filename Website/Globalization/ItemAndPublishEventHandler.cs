using System;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Modules.WeBlog.Diagnostics;

namespace Sitecore.Modules.WeBlog.Globalization
{
    public class ItemAndPublishEventHandler
    {
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
            if (item != null && item.TemplateID == Settings.DictionaryEntryTemplateID)
            {
                Logger.Info("Dictionary Entry saved, clearing Translator cache", this);
                Translator.ClearCaches();
            }
        }

        public void OnPublishEnd(object sender, EventArgs args)
        {
            Logger.Info("Publish complete, clearing Translator cache", this);
            Translator.ClearCaches();
        }
    }
}
