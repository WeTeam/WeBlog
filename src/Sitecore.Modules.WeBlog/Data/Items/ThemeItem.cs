using System;
using System.Collections.Generic;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Fields;
using Sitecore.Modules.WeBlog.Extensions;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class ThemeItem : CustomItem
    {
        public IEnumerable<FileItem> Stylesheets { get; protected set; }

        public IEnumerable<ScriptItem> Scripts { get; protected set; }

        private BaseTemplateManager _templateManager = null;

        [Obsolete("Use ctor(Item, BaseTemplateManager) instead.")]
        public ThemeItem(Item innerItem)
            : this(innerItem, ServiceLocator.ServiceProvider.GetService(typeof(BaseTemplateManager)) as BaseTemplateManager)
        {
        }

        public ThemeItem(Item innerItem, BaseTemplateManager templateManager)
            : base(innerItem)
        {
            Assert.ArgumentNotNull(templateManager, nameof(templateManager));
            _templateManager = templateManager;

            ResolveThemeAssets();
        }

        public string Credits
        {
            get { return InnerItem["Credit Markup"]; }
        }

        public static implicit operator ThemeItem(Item innerItem)
        {
            return innerItem != null ? new ThemeItem(innerItem) : null;
        }

        public static implicit operator Item(ThemeItem customItem)
        {
            return customItem != null ? customItem.InnerItem : null;
        }

        // to be removed once 2016 themes are done
        public CustomTextField FileLocation
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["File Location"]); }
        }

        protected void ResolveThemeAssets()
        {
            var scriptList = new List<ScriptItem>();
            var stylesheetList = new List<FileItem>();

            var children = InnerItem.GetChildren();

            foreach(Item item in children)
            {
                if (item.TemplateIsOrBasedOn(_templateManager, ScriptItem.TemplateId))
                    scriptList.Add(new ScriptItem(item));

                if (item.TemplateIsOrBasedOn(_templateManager, StylesheetItem.TemplateId))
                    stylesheetList.Add(new StylesheetItem(item));
            }

            Scripts = scriptList;
            Stylesheets = stylesheetList;
        }        
    }
}