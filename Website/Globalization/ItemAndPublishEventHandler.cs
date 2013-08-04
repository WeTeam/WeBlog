using System;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;

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

#if SC64 || SC66
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
#endif

        protected void HandleOnItemSaved(Item item)
        {
            if (item != null && item.TemplateID == Settings.DictionaryEntryTemplateID)
            {
                Log.Info("Dictionary Entry saved, clearing Translator cache", this);
                Translator.ClearCaches();
            }
        }

        public void OnPublishEnd(object sender, EventArgs args)
        {
            Log.Info("Publish complete, clearing Translator cache", this);
            Translator.ClearCaches();
        }
    }
}
