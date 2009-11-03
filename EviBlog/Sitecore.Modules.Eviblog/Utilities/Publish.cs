using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Sitecore.Data;

namespace Sitecore.Modules.Eviblog.Utilities
{
    class Publish
    {
        /// <summary>
        /// Publishes the item.
        /// </summary>
        /// <param name="targetItemID">The target item ID.</param>
        public static void PublishItem(ID targetItemID)
        {
            DateTime publishDate = DateTime.Now; 
            Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master"); 
            Sitecore.Data.Items.Item home = master.GetItem(targetItemID); 
            Sitecore.Data.Items.Item targets = master.GetItem("/sitecore/system/publishing targets");
            foreach (Sitecore.Data.Items.Item target in targets.Children)
            {
                string targetDBName = target["target database"];
                Sitecore.Data.Database targetDB = Sitecore.Configuration.Factory.GetDatabase(targetDBName);
                foreach (Sitecore.Globalization.Language language in master.Languages)
                {
                    Sitecore.Publishing.PublishOptions publishOptions = new Sitecore.Publishing.PublishOptions(master, targetDB, Sitecore.Publishing.PublishMode.SingleItem, language, publishDate);
                    publishOptions.RootItem = home;
                    publishOptions.Deep = false;
                    Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions); publisher.Publish();
                }
            }
        }

        /// <summary>
        /// Publishes the item.
        /// </summary>
        /// <param name="targetItemID">The target item ID.</param>
        public static void PublishItem(ID targetItemID, bool childs)
        {
            DateTime publishDate = DateTime.Now;
            Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
            Sitecore.Data.Items.Item home = master.GetItem(targetItemID);
            Sitecore.Data.Items.Item targets = master.GetItem("/sitecore/system/publishing targets");
            foreach (Sitecore.Data.Items.Item target in targets.Children)
            {
                string targetDBName = target["target database"];
                Sitecore.Data.Database targetDB = Sitecore.Configuration.Factory.GetDatabase(targetDBName);
                foreach (Sitecore.Globalization.Language language in master.Languages)
                {
                    Sitecore.Publishing.PublishOptions publishOptions = new Sitecore.Publishing.PublishOptions(master, targetDB, Sitecore.Publishing.PublishMode.SingleItem, language, publishDate);
                    publishOptions.RootItem = home;
                    publishOptions.Deep = childs;
                    Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions); publisher.Publish();
                }
            }
        }

        /// <summary>
        /// Publishes the item.
        /// </summary>
        /// <param name="targetItem">The target item.</param>
        public static void PublishItem(Item targetItem)
        {
            DateTime publishDate = DateTime.Now;
            Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
            Sitecore.Data.Items.Item home = targetItem;
            Sitecore.Data.Items.Item targets = master.GetItem("/sitecore/system/publishing targets");
            foreach (Sitecore.Data.Items.Item target in targets.Children)
            {
                string targetDBName = target["target database"];
                Sitecore.Data.Database targetDB = Sitecore.Configuration.Factory.GetDatabase(targetDBName);
                foreach (Sitecore.Globalization.Language language in master.Languages)
                {
                    Sitecore.Publishing.PublishOptions publishOptions = new Sitecore.Publishing.PublishOptions(master, targetDB, Sitecore.Publishing.PublishMode.SingleItem, language, publishDate);
                    publishOptions.RootItem = home;
                    publishOptions.Deep = false;
                    Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions); publisher.Publish();
                }
            }
        }

        /// <summary>
        /// Publishes the item.
        /// </summary>
        /// <param name="targetItem">The target item.</param>
        public static void PublishItem(Item targetItem, bool childs)
        {
            DateTime publishDate = DateTime.Now;
            Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
            Sitecore.Data.Items.Item home = targetItem;
            Sitecore.Data.Items.Item targets = master.GetItem("/sitecore/system/publishing targets");
            foreach (Sitecore.Data.Items.Item target in targets.Children)
            {
                string targetDBName = target["target database"];
                Sitecore.Data.Database targetDB = Sitecore.Configuration.Factory.GetDatabase(targetDBName);
                foreach (Sitecore.Globalization.Language language in master.Languages)
                {
                    Sitecore.Publishing.PublishOptions publishOptions = new Sitecore.Publishing.PublishOptions(master, targetDB, Sitecore.Publishing.PublishMode.SingleItem, language, publishDate);
                    publishOptions.RootItem = home;
                    publishOptions.Deep = childs;
                    Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions); publisher.Publish();
                }
            }
        }
    }
}
