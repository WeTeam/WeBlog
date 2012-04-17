using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.WeBlog.Globalization
{
    public class ItemAndPublishEventHandler
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            Item item = Event.ExtractParameter(args, 0) as Item;
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
