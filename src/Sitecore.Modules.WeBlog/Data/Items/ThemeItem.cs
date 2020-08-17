using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Modules.WeBlog.Data.Fields;
using System;
using System.Collections.Generic;

namespace Sitecore.Modules.WeBlog.Data.Items
{
    public class ThemeItem : CustomItem
    {
        [Obsolete("Use Sitecore.Modules.WeBlog.Themes.IThemeFileResolver from services instead.")]
        public IEnumerable<FileItem> Stylesheets { get; protected set; }

        [Obsolete("Use Sitecore.Modules.WeBlog.Themes.IThemeFileResolver from services instead.")]
        public IEnumerable<ScriptItem> Scripts { get; protected set; }

        public ThemeItem(Item innerItem)
            : base(innerItem)
        {
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

        [Obsolete("Use Sitecore.Modules.WeBlog.Themes.IThemeFileResolver from services instead.")]
        public CustomTextField FileLocation
        {
            get { return new CustomTextField(InnerItem, InnerItem.Fields["File Location"]); }
        }
        
        [Obsolete("No longer used.")]
        protected void ResolveThemeAssets()
        {
            Log.Warn("ResolveThemeAssets() was called on Sitecore.Modules.WeBlog.Data.Items.ThemeItem but it's no longer used.", this);
        }
    }
}